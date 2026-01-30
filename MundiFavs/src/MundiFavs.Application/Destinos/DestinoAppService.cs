using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core; // Necesario para ordenamiento dinámico
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using MundiFavs.CitySearch;

namespace MundiFavs.Destinos;

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

    public DestinoAppService(
        IDestinoRepository repository,
        ICitySearchService citySearchService)
        : base(repository)
    {
        _destinoRepository = repository;
        _citySearchService = citySearchService;
    }

    // --- NUEVO: Evitamos duplicados y asignamos manualmente Coordenadas/Población ---
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
}