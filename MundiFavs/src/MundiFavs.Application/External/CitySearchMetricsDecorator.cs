using MundiFavs.ApiMetrics;
using MundiFavs.CitySearch;
using MundiFavs.Destinos;
using MundiFavs.External;
using MundiFavs.External.CitySearch;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MundiFavs.External
{
    // Usamos ITransientDependency para que ABP lo registre. 
    // La interfaz ICitySearchService debe estar en el namespace MundiFavs.Destinos según tus archivos.
    public class CitySearchMetricsDecorator : ICitySearchService, ITransientDependency
    {
        private readonly GeoDbCitySearchService _innerService;
        private readonly ApiMetricManager _metricManager;

        public CitySearchMetricsDecorator(
            GeoDbCitySearchService innerService,
            ApiMetricManager metricManager)
        {
            _innerService = innerService;
            _metricManager = metricManager;
        }

        // 1. Implementación de búsqueda de ciudades
        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto input)
        {
            return await ExecuteWithMetricsAsync<CitySearchResultDto>(
                "SearchCitiesAsync",
                () => _innerService.SearchCitiesAsync(input)
            );
        }

        // 2. Implementación de detalle de ciudad (CORREGIDO: Ahora recibe el DTO)
        public async Task<CityDetailDto> GetCityDetailById(CityDetailRequestDto input)
        {
            return await ExecuteWithMetricsAsync<CityDetailDto>(
                "GetCityDetailById",
                () => _innerService.GetCityDetailById(input)
            );
        }

        // Método genérico para procesar las métricas
        private async Task<T> ExecuteWithMetricsAsync<T>(string endpoint, Func<Task<T>> action)
        {
            var watch = Stopwatch.StartNew();
            var isSuccess = true;
            var statusCode = 200;

            try
            {
                return await action();
            }
            catch (Exception)
            {
                isSuccess = false;
                statusCode = 500;
                throw;
            }
            finally
            {
                watch.Stop();
                // Grabamos en la tabla ApiMetrics
                await _metricManager.LogMetricAsync(
                    "GeoDB Cities",
                    endpoint,
                    (int)watch.ElapsedMilliseconds,
                    statusCode,
                    isSuccess);
            }
        }
    }
}