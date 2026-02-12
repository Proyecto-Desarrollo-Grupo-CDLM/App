using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MundiFavs.Calificaciones;
using MundiFavs.CitySearch;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace MundiFavs.Destinos;

[Authorize]
public class DestinoAppService :
    CrudAppService<
        Destino,
        DestinoDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinoDto>,
    IDestinoAppService
{
    private readonly ICitySearchService _citySearchService;
    private readonly IDestinoRepository _destinoRepository;
    private readonly IRepository<Calificacion, Guid> _calificacionRepository;
    private readonly IRepository<IdentityUser, Guid> _userRepository;

    // Repositorios para Eventos y Notificaciones
    private readonly IRepository<Evento, Guid> _eventoRepository;
    private readonly INotificacionAppService _notificacionService;

    public DestinoAppService(
        IDestinoRepository repository,
        IRepository<Calificacion, Guid> calificacionRepository,
        IRepository<IdentityUser, Guid> userRepository,
        ICitySearchService citySearchService,
        IRepository<Evento, Guid> eventoRepository,
        INotificacionAppService notificacionService
        )
        : base(repository)
    {
        _destinoRepository = repository;
        _citySearchService = citySearchService;
        _calificacionRepository = calificacionRepository;
        _userRepository = userRepository;
        _eventoRepository = eventoRepository;
        _notificacionService = notificacionService;
    }

    public override async Task<DestinoDto> CreateAsync(CreateUpdateDestinoDto input)
    {
        // 1. Validar Duplicados
        var query = await _destinoRepository.GetQueryableAsync();
        var currentUserId = CurrentUser.Id;

        var existe = await AsyncExecuter.AnyAsync(query, d =>
            d.Nombre == input.Nombre &&
            d.CreatorId == currentUserId
        );

        if (existe)
        {
            throw new UserFriendlyException($"¡Ya tienes guardada la ciudad '{input.Nombre}' en tus favoritos!");
        }

        // 2. Crear la entidad MANUALMENTE
        var nuevoDestino = new Destino(
            id: GuidGenerator.Create(),
            nombre: input.Nombre,
            pais: input.Pais,
            ciudad: input.Ciudad,
            poblacion: input.Poblacion,
            ubicacion: new Coordenadas(input.Latitud, input.Longitud),
            imageUrl: new Uri(input.ImageUrl)
        );
        nuevoDestino.SetExternalId(input.ExternalId);

        // 3. Guardar en Base de Datos
        await _destinoRepository.InsertAsync(nuevoDestino);

        return ObjectMapper.Map<Destino, DestinoDto>(nuevoDestino);
    }

    // --- NUEVO: Sobreescribimos UpdateAsync para notificar cambios en el destino (Req 7.2) ---
    public override async Task<DestinoDto> UpdateAsync(Guid id, CreateUpdateDestinoDto input)
    {
        // 1. Obtenemos el destino ANTES de actualizar
        var destinoOriginal = await _destinoRepository.GetAsync(id);
        int poblacionAnterior = destinoOriginal.Poblacion;

        // 2. Ejecutamos la actualización normal
        var destinoActualizadoDto = await base.UpdateAsync(id, input);

        // 3. Verificamos si hubo un cambio relevante (Ej: Población)
        if (poblacionAnterior != input.Poblacion)
        {
            var usuariosInteresados = await ObtenerUsuariosInteresados(id);

            foreach (var userId in usuariosInteresados)
            {
                // Evitamos notificar al mismo usuario que hizo el cambio (si aplica)
                if (userId != CurrentUser.Id)
                {
                    await _notificacionService.CrearNotificacionInternaAsync(
                        usuarioId: userId,
                        tituloDestino: destinoOriginal.Nombre,
                        cambioDetectado: $"La población ha cambiado de {poblacionAnterior} a {input.Poblacion} habitantes."
                    );
                }
            }
        }

        return destinoActualizadoDto;
    }

    // --- REQ 7.2: Notificar sobre Eventos ---
    public async Task<EventoDto> CrearEventoAsync(CreateEventoDto input)
    {
        // 1. Crear el evento
        var evento = new Evento(
            GuidGenerator.Create(),
            input.DestinoId,
            input.Titulo,
            input.Descripcion,
            input.FechaInicio,
            input.FechaFin
        );

        await _eventoRepository.InsertAsync(evento, autoSave: true);

        // 2. Lógica de Notificación
        var destino = await _destinoRepository.GetAsync(input.DestinoId);
        var usuariosInteresados = await ObtenerUsuariosInteresados(input.DestinoId);

        foreach (var userId in usuariosInteresados)
        {
            // Solo notificamos si no es el mismo que creó el evento
            if (userId != CurrentUser.Id)
            {
                // ADAPTACIÓN AL DIAGRAMA:
                // TituloDestino -> Nombre del destino
                // CambioDetectado -> Descripción del evento
                await _notificacionService.CrearNotificacionInternaAsync(
                    usuarioId: userId,
                    tituloDestino: destino.Nombre,
                    cambioDetectado: $"Nuevo evento: {input.Titulo}. {input.Descripcion.Substring(0, Math.Min(30, input.Descripcion.Length))}..."
                );
            }
        }

        return ObjectMapper.Map<Evento, EventoDto>(evento);
    }

    // Método auxiliar para saber a quién notificar
    private async Task<List<Guid>> ObtenerUsuariosInteresados(Guid destinoId)
    {
        // Estrategia: Notificar a todos los que han dejado reseña en este destino
        var query = await _calificacionRepository.GetQueryableAsync();

        var userIds = await query
            .Where(c => c.DestinoId == destinoId)
            .Select(c => c.UserId)
            .Distinct()
            .ToListAsync();

        return userIds;
    }

    // --- Operación 3.1 & 3.2: Buscar ciudades (API Externa) ---
    public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
        return await _citySearchService.SearchCitiesAsync(request);
    }

    // --- Operación 3.4: Listar populares (Base de Datos Local) ---
    public async Task<List<DestinoDto>> GetPopularDestinationsAsync(int maxCount = 10)
    {
        var destinosPopulares = await _destinoRepository.GetPopularAsync(maxCount);
        return ObjectMapper.Map<List<Destino>, List<DestinoDto>>(destinosPopulares);
    }

    // --- Operación 5: Mis Destinos Guardados ---
    public async Task<PagedResultDto<DestinoDto>> GetMyDestinationsAsync(PagedAndSortedResultRequestDto input)
    {
        var myUserId = CurrentUser.Id;
        if (myUserId == null)
        {
            throw new UserFriendlyException("Debes iniciar sesión para ver tus destinos.");
        }

        var query = await _destinoRepository.GetQueryableAsync();
        query = query.Where(d => d.CreatorId == myUserId);

        var totalCount = await AsyncExecuter.CountAsync(query);

        var sorting = input.Sorting;
        if (string.IsNullOrEmpty(sorting))
        {
            sorting = $"{nameof(Destino.CreationTime)} DESC";
        }
        query = query.OrderBy(sorting);

        query = query.PageBy(input);

        var entities = await AsyncExecuter.ToListAsync(query);
        var dtos = ObjectMapper.Map<List<Destino>, List<DestinoDto>>(entities);
        return new PagedResultDto<DestinoDto>(totalCount, dtos);
    }

    // --- Operacion 5.5: Obtener comentarios ---
    public async Task<DestinoComentariosDto> GetComentariosConPromedioAsync(string externalCityId)
    {
        Logger.LogInformation($"🔍 Buscando comentarios para externalCityId: '{externalCityId}'");

        if (string.IsNullOrWhiteSpace(externalCityId))
        {
            return new DestinoComentariosDto { Comentarios = new List<ComentarioDto>() };
        }

        var idLimpio = externalCityId.Trim();
        var destino = await _destinoRepository.FirstOrDefaultAsync(d => d.ExternalId == idLimpio);

        if (destino == null)
        {
            return new DestinoComentariosDto
            {
                Comentarios = new List<ComentarioDto>(),
                NombreDestino = "Destino no guardado aún"
            };
        }

        var queryableCalificaciones = await _calificacionRepository.GetQueryableAsync();
        var calificaciones = await queryableCalificaciones
            .Where(c => c.DestinoId == destino.Id)
            .OrderByDescending(c => c.CreationTime)
            .Take(50)
            .ToListAsync();

        if (!calificaciones.Any())
        {
            return new DestinoComentariosDto
            {
                DestinoId = destino.Id,
                NombreDestino = destino.Nombre,
                PuntuacionPromedio = 0,
                TotalCalificaciones = 0,
                Comentarios = new List<ComentarioDto>()
            };
        }

        var userIds = calificaciones.Select(c => c.UserId).Distinct().ToList();
        var queryableUsers = await _userRepository.GetQueryableAsync();
        var usuariosDict = await queryableUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Usuario Desconocido");

        var promedio = calificaciones.Average(c => c.Estrellas);

        var comentariosDto = calificaciones.Select(c => new ComentarioDto
        {
            Id = c.Id,
            Estrellas = c.Estrellas,
            Comentario = c.Comentario,
            CreationTime = c.CreationTime,
            AutorNombre = usuariosDict.GetValueOrDefault(c.UserId, "Usuario Desconocido")
        }).ToList();

        return new DestinoComentariosDto
        {
            DestinoId = destino.Id,
            NombreDestino = destino.Nombre,
            PuntuacionPromedio = Math.Round(promedio, 1),
            TotalCalificaciones = calificaciones.Count,
            Comentarios = comentariosDto
        };
    }
}