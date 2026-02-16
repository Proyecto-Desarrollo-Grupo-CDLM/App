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

    public override async Task<DestinoDto> UpdateAsync(Guid id, CreateUpdateDestinoDto input)
    {
        // 1. Obtenemos la entidad original antes de que ABP la modifique
        // Esto es vital para tener los datos de ubicación "seguros" para la búsqueda
        var destinoOriginal = await _destinoRepository.GetAsync(id);

        // Guardamos el estado anterior para comparar
        int poblacionAnterior = destinoOriginal.Poblacion;

        // Guardamos las coordenadas lógicas (Clave Natural) para buscar a los otros interesados
        string nombreCiudad = destinoOriginal.Nombre;
        string pais = destinoOriginal.Pais;
        string provincia = destinoOriginal.Ciudad;

        // 2. Ejecutamos la actualización normal (base.UpdateAsync persiste los cambios en ESTE destino)
        var destinoActualizadoDto = await base.UpdateAsync(id, input);

        // 3. Verificamos si hubo un cambio relevante (Ej: Población)
        if (poblacionAnterior != input.Poblacion)
        {
            // CORRECCIÓN IMPORTANTE:
            // No buscamos por 'id' porque ese ID es único de tu usuario.
            // Buscamos por la combinación geográfica usando el método que modificamos anteriormente.
            var usuariosInteresados = await ObtenerUsuariosInteresados(nombreCiudad, pais, provincia);

            foreach (var userId in usuariosInteresados)
            {
                // Evitamos notificar al mismo usuario que hizo el cambio
                if (userId != CurrentUser.Id)
                {
                    await _notificacionService.CrearNotificacionInternaAsync(
                        usuarioId: userId,
                        tituloDestino: nombreCiudad,
                        cambioDetectado: $"Actualización en {nombreCiudad}: La población ha cambiado de {poblacionAnterior} a {input.Poblacion} habitantes."
                    );
                }
            }
        }

        return destinoActualizadoDto;
    }

    // --- REQ 7.2: Notificar sobre Eventos ---
    public async Task<EventoDto> CrearEventoAsync(CreateEventoDto input)
    {
        // 1. Recuperar el destino del creador para obtener los datos geográficos únicos
        var destinoCreador = await _destinoRepository.GetAsync(input.DestinoId);

        // 2. Crear el evento vinculado al destino del creador
        var evento = new Evento(
            GuidGenerator.Create(),
            input.DestinoId,
            input.Titulo,
            input.Descripcion,
            input.FechaInicio,
            input.FechaFin
        );

        await _eventoRepository.InsertAsync(evento, autoSave: true);

        // 3. Lógica de Notificación basada en ubicación geográfica (Ciudad + País + Provincia)
        var usuariosInteresados = await ObtenerUsuariosInteresados(
            destinoCreador.Nombre,
            destinoCreador.Pais,
            destinoCreador.Ciudad
        );

        foreach (var userId in usuariosInteresados)
        {
            // Solo notificamos si no es el mismo que creó el evento
            if (userId != CurrentUser.Id)
            {
                await _notificacionService.CrearNotificacionInternaAsync(
                    usuarioId: userId,
                    tituloDestino: destinoCreador.Nombre,
                    cambioDetectado: $"Nuevo evento en {destinoCreador.Nombre}: {input.Titulo}."
                );
            }
        }

        return ObjectMapper.Map<Evento, EventoDto>(evento);
    }

    // Método auxiliar modificado para buscar por atributos geográficos
    private async Task<List<Guid>> ObtenerUsuariosInteresados(string ciudad, string pais, string provincia)
    {
        var queryDestinos = await _destinoRepository.GetQueryableAsync();

        // Buscamos todos los destinos que coincidan geográficamente, 
        // sin importar quién sea el dueño (CreatorId / UserId)
        var userIds = await queryDestinos
            .Where(d => d.Nombre == ciudad &&
                        d.Pais == pais &&
                        d.Ciudad == provincia)
            .Select(d => d.CreatorId) // O UserId, dependiendo de cómo implementaste IUserOwned
            .Where(id => id.HasValue)
            .Select(id => id.Value)
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


    public async Task<DestinoComentariosDto> GetComentariosConPromedioAsync(string externalCityId)
    {
        Logger.LogInformation($"🔍 Buscando comentarios globales para externalCityId: '{externalCityId}'");

        if (string.IsNullOrWhiteSpace(externalCityId))
        {
            return new DestinoComentariosDto { Comentarios = new List<ComentarioDto>() };
        }

        var idLimpio = externalCityId.Trim();

        var queryableDestinos = await _destinoRepository.GetQueryableAsync();

        var destinosEncontrados = await queryableDestinos
            .Where(d => d.ExternalId == idLimpio)
            .Select(d => new { d.Id, d.Nombre }) 
            .ToListAsync();

        if (!destinosEncontrados.Any())
        {
            return new DestinoComentariosDto
            {
                Comentarios = new List<ComentarioDto>(),
                NombreDestino = "Destino no encontrado"
            };
        }

        
        var listaDeIds = destinosEncontrados.Select(d => d.Id).ToList();

        
        var nombreCiudad = destinosEncontrados.First().Nombre;

        
        var queryableCalificaciones = await _calificacionRepository.GetQueryableAsync();

        var calificaciones = await queryableCalificaciones
            .Where(c => listaDeIds.Contains(c.DestinoId))
            .OrderByDescending(c => c.CreationTime)
            .Take(50)
            .ToListAsync();

        
        if (!calificaciones.Any())
        {
            return new DestinoComentariosDto
            {
                
                DestinoId = listaDeIds.First(),
                NombreDestino = nombreCiudad,
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
            DestinoId = listaDeIds.First(), 
            NombreDestino = nombreCiudad,
            PuntuacionPromedio = Math.Round(promedio, 1),
            TotalCalificaciones = calificaciones.Count,
            Comentarios = comentariosDto
        };
    }
}