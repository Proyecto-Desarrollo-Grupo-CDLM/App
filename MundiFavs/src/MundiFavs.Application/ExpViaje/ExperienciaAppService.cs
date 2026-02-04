using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Necesario para consultas dinámicas
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace MundiFavs.Experiencias
{
    [Authorize] // Requiere estar logueado por defecto
    public class ExperienciaAppService : MundiFavsAppService, IExperienciaAppService
    {
        private readonly IRepository<Experiencia, Guid> _experienciaRepository;

        public ExperienciaAppService(IRepository<Experiencia, Guid> experienciaRepository)
        {
            _experienciaRepository = experienciaRepository;
        }

        // 4.1. Crear nueva experiencia
        public async Task<ExperienciaDto> CreateAsync(CreateUpdateExperienciaDto input)
        {
            var userId = CurrentUser.GetId();

            // Usamos el constructor de tu entidad respetando el orden de parámetros:
            // (id, userId, destinoId, comentario, valoracion, etiquetas, fechaExperiencia)
            var experiencia = new Experiencia(
                GuidGenerator.Create(),
                userId,
                input.DestinoId,
                input.Comentario,
                input.Valoracion,
                input.Etiquetas,
                input.FechaExperiencia
            );

            await _experienciaRepository.InsertAsync(experiencia);
            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experiencia);
        }

        // 4.2. Editar experiencia propia
        public async Task<ExperienciaDto> UpdateAsync(Guid id, CreateUpdateExperienciaDto input)
        {
            var experiencia = await _experienciaRepository.GetAsync(id);

            // Validación de seguridad: Usamos UserdId (con la 'd' extra según tu entidad)
            if (experiencia.UserdId != CurrentUser.GetId())
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("No puedes editar una experiencia que no es tuya.");
            }

            // Actualizamos propiedades (Titulo ya no existe)
            experiencia.Comentario = input.Comentario;
            experiencia.Valoracion = input.Valoracion;
            experiencia.Etiquetas = input.Etiquetas;
            experiencia.FechaExperiencia = input.FechaExperiencia;

            await _experienciaRepository.UpdateAsync(experiencia);
            return ObjectMapper.Map<Experiencia, ExperienciaDto>(experiencia);
        }

        // 4.3. Eliminar experiencia propia
        public async Task DeleteAsync(Guid id)
        {
            var experiencia = await _experienciaRepository.GetAsync(id);

            // Validación de seguridad
            if (experiencia.UserdId != CurrentUser.GetId())
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("No puedes eliminar una experiencia que no es tuya.");
            }

            await _experienciaRepository.DeleteAsync(experiencia);
        }

        // 4.4, 4.5, 4.6 Consultar y Filtrar
        [AllowAnonymous]
        public async Task<PagedResultDto<ExperienciaDto>> GetListAsync(GetExperienciasInput input)
        {
            var query = await _experienciaRepository.GetQueryableAsync();

            // 1. Filtro por Destino (4.4)
            if (input.DestinoId.HasValue)
            {
                query = query.Where(x => x.DestinoId == input.DestinoId.Value);
            }

            // 2. Búsqueda por palabras clave (4.6)
            // Como ya no existe Titulo, buscamos en Comentario y Etiquetas
            if (!input.FiltroTexto.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Comentario.Contains(input.FiltroTexto) ||
                                         x.Etiquetas.Contains(input.FiltroTexto));
            }

            // 3. Filtrar por Valoración (4.5)
            // Asumimos que actualizaste GetExperienciasInput para tener 'Valoracion? Valoracion' (Enum)
            if (input.Valoracion.HasValue)
            {
                query = query.Where(x => x.Valoracion == input.Valoracion.Value);
            }

            // Paginación y Ordenamiento
            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderByDescending(x => x.CreationTime)
                         .PageBy(input);

            var experiencias = await AsyncExecuter.ToListAsync(query);

            var dtos = ObjectMapper.Map<List<Experiencia>, List<ExperienciaDto>>(experiencias);

            return new PagedResultDto<ExperienciaDto>(totalCount, dtos);
        }
    }
}