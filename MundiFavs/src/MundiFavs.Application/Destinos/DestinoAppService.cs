using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MundiFavs.Calificaciones;
using MundiFavs.CitySearch;
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

    public DestinoAppService(
        IDestinoRepository repository,
        IRepository<Calificacion, Guid> calificacionRepository,
        IRepository<IdentityUser, Guid> userRepository,
        ICitySearchService citySearchService)
        : base(repository)
    {
        _destinoRepository = repository;
        _citySearchService = citySearchService;
        _calificacionRepository = calificacionRepository;
        _userRepository = userRepository;
    }

  
    public override async Task<DestinoDto> CreateAsync(CreateUpdateDestinoDto input)
    {
        // 1. Validar Duplicados
        var query = await _destinoRepository.GetQueryableAsync();
        var currentUserId = CurrentUser.Id;

        // Verificamos si este usuario ya guardó esa ciudad
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

        // 4. Devolver el DTO mapeado
        return ObjectMapper.Map<Destino, DestinoDto>(nuevoDestino);
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

    // --- NUEVO MÉTODO (Operación 5): Mis Destinos Guardados ---
    public async Task<PagedResultDto<DestinoDto>> GetMyDestinationsAsync(PagedAndSortedResultRequestDto input)
    {
        // 1. Obtener ID del usuario actual
        var myUserId = CurrentUser.Id;
        if (myUserId == null)
        {
            throw new UserFriendlyException("Debes iniciar sesión para ver tus destinos.");
        }

        // 2. Obtener la Query base
        var query = await _destinoRepository.GetQueryableAsync();

        // 3. FILTRAR: Solo los creados por MÍ
        query = query.Where(d => d.CreatorId == myUserId);

        // 4. Contar total (para la paginación)
        var totalCount = await AsyncExecuter.CountAsync(query);

        // 5. Ordenar (Por defecto: Lo más reciente primero)
        query = query.OrderBy(input.Sorting ?? nameof(Destino.CreationTime) + " DESC");

        // 6. Paginar (Ej: Página 1, traer 10)
        query = query.PageBy(input);

        // 7. Ejecutar consulta
        var entities = await AsyncExecuter.ToListAsync(query);

        // 8. Convertir a DTO y devolver
        var dtos = ObjectMapper.Map<List<Destino>, List<DestinoDto>>(entities);
        return new PagedResultDto<DestinoDto>(totalCount, dtos);
    }

    

    // Obtiene comentarios y promedio de estrellas de una ciudad(Operacion 5.5)
    public async Task<DestinoComentariosDto> GetComentariosConPromedioAsync(string externalCityId)
    {
        Logger.LogInformation($"🔍 Buscando comentarios para externalCityId: '{externalCityId}'");

        // 1. Validación
        if (string.IsNullOrWhiteSpace(externalCityId))
        {
            Logger.LogWarning("⚠️ ExternalCityId está vacío");
            return new DestinoComentariosDto
            {
                Comentarios = new List<ComentarioDto>()
            };
        }

        var idLimpio = externalCityId.Trim();

        // 2. Buscar destino por ExternalId
        var destino = await _destinoRepository
            .FirstOrDefaultAsync(d => d.ExternalId == idLimpio);

        if (destino == null)
        {
            Logger.LogWarning($"⚠️ No se encontró destino con ExternalId: '{idLimpio}'");

            return new DestinoComentariosDto
            {
                Comentarios = new List<ComentarioDto>(),
                NombreDestino = "Destino no guardado aún"
            };
        }

        Logger.LogInformation($"✅ Destino encontrado: {destino.Nombre} (ID: {destino.Id})");

        // 3. Obtener calificaciones
        var queryableCalificaciones = await _calificacionRepository.GetQueryableAsync();

        var calificaciones = await queryableCalificaciones
            .Where(c => c.DestinoId == destino.Id)
            .OrderByDescending(c => c.CreationTime)
            .Take(50)
            .ToListAsync();

        Logger.LogInformation($"📊 Se encontraron {calificaciones.Count} calificaciones");

        // Si no hay calificaciones, retornar vacío
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

        // 4. Obtener usuarios
        var userIds = calificaciones.Select(c => c.UserId).Distinct().ToList();

        var queryableUsers = await _userRepository.GetQueryableAsync();
        var usuariosDict = await queryableUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "Usuario Desconocido");

        // 5. Calcular promedio
        var promedio = calificaciones.Average(c => c.Estrellas);

        // 6. Mapear a DTOs
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
