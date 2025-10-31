// En CalificacionAppService.cs

using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using MundiFavs.Destinos; // <-- AÑADIR USING DE DESTINOS
using Volo.Abp.Guids; // <-- AÑADIR USING DE GUIDGENERATOR

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
        
        // --- PASO 1: INYECTAR DEPENDENCIAS NECESARIAS ---
        private readonly IRepository<Destino, Guid> _destinoRepository;
        private readonly IGuidGenerator _guidGenerator;

        public CalificacionAppService(
            IRepository<Calificacion, Guid> repository,
            ICurrentUser currentUser,
            IRepository<Destino, Guid> destinoRepository, // <--- Inyectar Repo de Destino
            IGuidGenerator guidGenerator) // <--- Inyectar Generador de Guid
            : base(repository)
        {
            _currentUser = currentUser;
            _destinoRepository = destinoRepository; // <--- Asignar
            _guidGenerator = guidGenerator; // <--- Asignar
        }

        // --- PASO 2: SOBRESCRIBIR CREATEASYNC CON LÓGICA MANUAL ---
        public override async Task<CalificacionDto> CreateAsync(CreateUpdateCalificacionDto input)
        {
            // 1. Obtener el UserId (ya lo tenías)
            var userId = _currentUser.Id.Value;

            // 2. Obtener la entidad Destino (REQUERIDO POR EL CONSTRUCTOR)
            var destino = await _destinoRepository.GetAsync(input.DestinoId);
            
            // 3. Generar un nuevo Id para la Calificacion
            var calificacionId = _guidGenerator.Create();

            // 4. Crear la entidad Calificacion usando su constructor
            var calificacion = new Calificacion(
                calificacionId,
                input.Estrellas,
                input.Comentario,
                destino, // Pasamos la entidad Destino
                userId     // Pasamos el UserId
            );

            // 5. Guardar la entidad en la base de datos
            await Repository.InsertAsync(calificacion, autoSave: true);

            // 6. Mapear la entidad guardada al DTO de salida
            return await MapToGetOutputDtoAsync(calificacion);
        }
    }
}