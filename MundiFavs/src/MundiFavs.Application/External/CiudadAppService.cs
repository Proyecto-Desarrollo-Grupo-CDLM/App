using Microsoft.AspNetCore.Authorization;
using MundiFavs.CitySearch;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

namespace MundiFavs.Application.CitySearch
{
[Authorize]
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

        public async Task<CityDetailDto> GetCityDetail(CityDetailRequestDto input)
        {
            // 1. Validación de entrada (mínima lógica de negocio)
            if (input == null || string.IsNullOrWhiteSpace(input.CityId))
            {
                throw new UserFriendlyException("Se requiere el identificador de la ciudad (CityId).");
            }

            // 2. Orquestación: Delegar la tarea al servicio de infraestructura
            // El AppService se asegura de que la interfaz de la API externa lo maneje.
            var cityDetail = await _citySearchService.GetCityDetailById(input);

            // 3. Devolución de resultados
            // Si el objeto cityDetail es null (lo cual debería ser manejado por GeoDbCitySearchService con una excepción, pero como chequeo extra):
            if (cityDetail == null)
            {
                throw new EntityNotFoundException($"No se pudo obtener el detalle de la ciudad con ID: {input.CityId}.");
            }

            return cityDetail;
        }
    }
}