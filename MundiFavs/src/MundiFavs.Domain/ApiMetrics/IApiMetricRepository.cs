using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace MundiFavs.Domain.ApiMetrics
{
    public interface IApiMetricRepository : IRepository<ApiMetric, Guid>
    {
        // Obtener métricas por rango de fechas
        Task<List<ApiMetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Obtener métricas de un endpoint específico
        Task<List<ApiMetric>> GetByEndpointAsync(string endpoint, int maxResults = 100);

        // Obtener solo las métricas que tuvieron errores
        Task<List<ApiMetric>> GetErrorMetricsAsync(DateTime startDate, DateTime endDate);

        // Contar cuántas llamadas se hicieron a cada endpoint
        Task<Dictionary<string, int>> GetCallCountByEndpointAsync(DateTime startDate, DateTime endDate);

        // Obtener el tiempo promedio de respuesta por endpoint
        Task<Dictionary<string, double>> GetAverageResponseTimeByEndpointAsync(DateTime startDate, DateTime endDate);

        // Obtener el total de llamadas en un período
        Task<int> GetTotalCallsAsync(DateTime startDate, DateTime endDate);

        // Obtener la tasa de error (porcentaje de llamadas fallidas)
        Task<double> GetErrorRateAsync(DateTime startDate, DateTime endDate);
    }
}