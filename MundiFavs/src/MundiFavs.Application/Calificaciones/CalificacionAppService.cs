using Microsoft.AspNetCore.Authorization;
using MundiFavs.Destinos;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace MundiFavs.Calificaciones
{
    [Authorize]
    public class CalificacionAppService :
        CrudAppService<
            Calificacion,
            CalificacionDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateCalificacionDto>,
        ICalificacionAppService
       
        
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IRepository<Calificacion, Guid> _calificacionRepository;

        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            ICurrentUser currentUser,
            IRepository<Destino, Guid> destinoRepository,
            IGuidGenerator guidGenerator)
            : base(repository)
        {
            _currentUser = currentUser;
            _destinoRepository = destinoRepository;
            _guidGenerator = guidGenerator;
            _calificacionRepository= repository;
        }

        
        public Lazy<IObjectMapper> ObjectMapperLazy { get; internal set; }

        public override async Task<CalificacionDto> CreateAsync(CreateUpdateCalificacionDto input)
        {
            var userId = _currentUser.Id.Value;

            // Verificar duplicado
            var calificacionExistente = await Repository.FirstOrDefaultAsync(
                c => c.UserId == userId && c.DestinoId == input.DestinoId
            );

            if (calificacionExistente != null)
                throw new UserFriendlyException("Ya has calificado este destino.");

            // Obtener destino
            var destino = await _destinoRepository.GetAsync(input.DestinoId);

            // Crear nueva entidad
            var calificacionId = _guidGenerator.Create();

            var calificacion = new Calificacion(
                calificacionId,
                input.Estrellas,
                input.Comentario,
                destino,
                userId
            );

            await Repository.InsertAsync(calificacion, autoSave: true);

            // Usa nuestro mapper manual en lugar del de ABP
            return await MapToGetOutputDtoAsync(calificacion);
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



