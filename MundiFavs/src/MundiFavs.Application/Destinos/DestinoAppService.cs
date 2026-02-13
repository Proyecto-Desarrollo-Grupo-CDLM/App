using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp; // <--- NECESARIO para UserFriendlyException
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

    // --- NUEVO: Evitamos duplicados al guardar ---
    // --- NUEVO: Evitamos duplicados al guardar ---
    public override async Task<DestinoDto> CreateAsync(CreateUpdateDestinoDto input)
    {
        // 1. Obtenemos el acceso a la consulta
        var query = await _destinoRepository.GetQueryableAsync();

        // [CORRECCIÓN CLAVE]: Extraemos el ID a una variable local ANTES de la consulta.
        // EF Core no puede traducir "CurrentUser.Id" dentro del AnyAsync, pero sí entiende una variable Guid?.
        var currentUserId = CurrentUser.Id;

        // 2. Verificamos si YA existe un destino con el mismo nombre para ESTE usuario
        var existe = await AsyncExecuter.AnyAsync(query, d =>
            d.Nombre == input.Nombre &&
            d.CreatorId == currentUserId // Usamos la variable local
        );

        if (existe)
        {
            // 3. Si existe, lanzamos error (Angular mostrará esto en rojo)
            throw new UserFriendlyException($"¡Ya tienes guardada la ciudad '{input.Nombre}' en tus favoritos!");
        }

        // 4. Si no existe, procedemos a crear normalmente
        return await base.CreateAsync(input);
    }

    // --- Operación 3.1 & 3.2: Buscar ciudades (API Externa) ---
    public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
        return await _citySearchService.SearchCitiesAsync(request);
    }

    // --- Operación 3.4: Listar populares (Base de Datos Local) ---
    public async Task<List<DestinoDto>> GetPopularDestinationsAsync(int maxCount = 10)
    {
        // 1. Obtener las entidades desde el repositorio
        var destinosPopulares = await _destinoRepository.GetPopularAsync(maxCount);

        // 2. Mapear automáticamente de Entidad (Destino) a DTO (DestinoDto)
        return ObjectMapper.Map<List<Destino>, List<DestinoDto>>(destinosPopulares);
    }
}