using MundiFavs.CitySearch;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using System.Collections.Generic;

namespace MundiFavs.Application.CitySearch;

// La interfaz ICitySearchService ya está en la capa de CitySearch (dominio/contratos).
// El AppService solo la usa.
public class CiudadAppService : ApplicationService // Hereda de ApplicationService de ABP
{
    // Inyectamos la interfaz que define la comunicación con el servicio externo.
    
    private readonly ICitySearchService _citySearchService;

    public CiudadAppService(ICitySearchService citySearchService)
    {
        _citySearchService = citySearchService;
    }

    /// <summary>
    /// Implementa la operación 3.1: Buscar ciudades por nombre.
    /// Delega la búsqueda al servicio externo y devuelve los DTOs.
    /// </summary>
    /// <param name="request">DTO con el prefijo de la ciudad a buscar.</param>
    /// <returns>DTO con la lista de ciudades encontradas.</returns>
    public async Task<CitySearchResultDto> SearchCitiesByNameAsync(CitySearchRequestDto request)
    {
        // 1. Validación de entrada (mínima lógica de negocio)
        if (request == null || string.IsNullOrWhiteSpace(request.NombreCiudad))
        {
            // Podrías lanzar una excepción o simplemente retornar una lista vacía si es un caso esperado.
            return new CitySearchResultDto
            {
                CityNames = new List<CiudadDto>() // Garantizar que la lista no sea null
            };
        }

        // 2. Orquestación: Delegar la tarea de comunicación externa al servicio de infraestructura
        // El AppService no sabe CÓMO se comunican, solo que el servicio ICitySearchService lo hará.
        var result = await _citySearchService.SearchCitiesAsync(request);

        // 3. Manejo y devolución de resultados
        // El DTO de respuesta ya viene mapeado correctamente desde GeoDbCitySearchService,
        // por lo que solo lo devolvemos. Si hubiera lógica adicional (filtrado, ordenamiento)
        // se aplicaría aquí.

        return result;
    }
}