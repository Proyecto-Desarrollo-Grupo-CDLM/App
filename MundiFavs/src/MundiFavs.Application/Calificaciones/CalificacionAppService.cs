using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MundiFavs.Destinos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;

namespace MundiFavs.Calificaciones
{
    [Authorize]
    public class CalificacionAppService : CrudAppService<
            Calificacion,
            CalificacionDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateCalificacionDto>,
        ICalificacionAppService
       
        
    {
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IRepository<Calificacion, Guid> _calificacionRepository;

        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            Volo.Abp.Users.ICurrentUser mockCurrentUser,
            IRepository<Destino, Guid> destinoRepository,
            Volo.Abp.Guids.IGuidGenerator mockGuidGenerator)
            : base(repository)
        {
            _calificacionRepository = repository;
            _destinoRepository = destinoRepository;
            _guidGenerator = _guidGenerator;
            _calificacionRepository= repository;
        }

        // --- 1. MÉTODO PARA VERIFICAR SI YA CALIFIQUÉ ---
        public async Task<CalificacionDto?> GetMyCalificacionAsync(Guid destinoId)
        {
            var query = await _calificacionRepository.GetQueryableAsync();

            // Buscamos una calificación que coincida con el Destino Y con el Usuario actual
            var calificacion = query.FirstOrDefault(x => x.DestinoId == destinoId && x.CreatorId == CurrentUser.Id);

            if (calificacion == null)
            {
                return null; // No existe
            }

            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        // --- 2. CREAR (CREATE) ---
        public override async Task<CalificacionDto> CreateAsync(CreateUpdateCalificacionDto input)
        {
            var destino = await _destinoRepository.GetAsync(input.DestinoId);

            // Aquí lo creas manualmente, por eso funcionaba bien al principio
            var nuevaCalificacion = new Calificacion(
                GuidGenerator.Create(),
                input.Puntuacion,
                input.Comentario,
                destino,
                CurrentUser.Id ?? Guid.Empty
            );

            await _calificacionRepository.InsertAsync(nuevaCalificacion, autoSave: true);

            // Recalculamos promedio
            await ActualizarPromedioDestinoAsync(input.DestinoId);

            return ObjectMapper.Map<Calificacion, CalificacionDto>(nuevaCalificacion);
        }

        // --- 3. ACTUALIZAR (UPDATE) ---
        // 👇 AQUÍ ESTÁ LA CORRECCIÓN CLAVE
        public override async Task<CalificacionDto> UpdateAsync(Guid id, CreateUpdateCalificacionDto input)
        {
            // 1. Buscamos la entidad original
            var calificacion = await _calificacionRepository.GetAsync(id);

            // 2. Asignamos MANUALMENTE las propiedades.
            // Esto soluciona el error: Como 'Puntuacion' (DTO) no se llama igual que 'Estrellas' (Entidad),
            // el update automático fallaba. Al hacerlo a mano, aseguramos el cambio.
            calificacion.ActualizarDatos(input.Puntuacion,input.Comentario);    

            // 3. Guardamos los cambios
            await _calificacionRepository.UpdateAsync(calificacion, autoSave: true);

            // 4. Recalculamos el promedio con el nuevo valor
            await ActualizarPromedioDestinoAsync(input.DestinoId);

            return ObjectMapper.Map<Calificacion, CalificacionDto>(calificacion);
        }

        // --- 4. MÉTODO PRIVADO (Lógica compartida) ---
        private async Task ActualizarPromedioDestinoAsync(Guid destinoId)
        {
            var query = await _calificacionRepository.GetQueryableAsync();

            double nuevoPromedio = 0;

            var calificacionesDestino = query.Where(x => x.DestinoId == destinoId);

            if (calificacionesDestino.Any())
            {
                nuevoPromedio = calificacionesDestino.Average(x => (double)x.Estrellas);
            }

            // Actualizamos el destino
            var destino = await _destinoRepository.GetAsync(destinoId);
            destino.SetPuntuacion(nuevoPromedio);

            await _destinoRepository.UpdateAsync(destino);
        }
    

        // 🔧 Override para evitar usar ObjectMapper del framework (que es null en tests)
       /* protected override Task<CalificacionDto> MapToGetOutputDtoAsync(Calificacion entity)
        {
            return Task.FromResult(ObjectMapperLazy.Value.Map<Calificacion, CalificacionDto>(entity));
        }*/




        [Authorize]
        
        public async Task<CalificacionDto> UpdateCalificacionAsync(Guid id, UpdateCalificacionDto input)
        {
            // 1. Obtener la entidad. Si usas filtros automáticos de ABP, esta consulta ya filtra por CurrentUser.Id.
            // PERO HACEMOS LA VERIFICACIÓN EXPLÍCITA COMO BUENA PRÁCTICA DE SEGURIDAD.
            var calificacion = await _calificacionRepository.GetAsync(id);

            // Seguridad: Chequear que la calificación es del usuario actual (doble check)
            if (calificacion.UserId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No está autorizado a modificar esta calificación. Solo el propietario puede hacerlo.");
            }

            // Comportamiento de Dominio: Llama al método de la entidad (protege Estrellas y Comentario)
            calificacion.Update(input.Estrellas, input.Comentario);

            // Persistir el cambio
            var updatedCalificacion = await _calificacionRepository.UpdateAsync(calificacion);

            // Mapear y retornar el DTO
            return ObjectMapper.Map<Calificacion, CalificacionDto>(updatedCalificacion);
        }

        [Authorize]
        public override async Task DeleteAsync(Guid id)
        {
            // 1. Obtener la calificación.
            var calificacion = await _calificacionRepository.GetAsync(id);

            // **2. Validación de Propiedad Explícita**
            if (calificacion.UserId != CurrentUser.Id)
            {
                throw new AbpAuthorizationException("No está autorizado a eliminar esta calificación. Solo el propietario puede hacerlo.");
            }

            // 3. Eliminar
            await _calificacionRepository.DeleteAsync(id);
        }
    }

}



