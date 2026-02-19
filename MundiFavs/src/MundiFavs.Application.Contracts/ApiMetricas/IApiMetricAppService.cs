using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MundiFavs.ApiMetrics
{
    public interface IApiMetricAppService : IApplicationService
    {
        // Obtiene la lista paginada de métricas
        Task<List<ApiMetricDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        // Un método extra para ver estadísticas rápidas
        Task<ApiSummaryDto> GetSummaryAsync();

        Task<List<ApiMetricDto>> GetFilteredListAsync(
        int? statusCode,
        string endpoint,
        DateTime? fromDate);

        Task DeleteOldMetricsAsync();
    }

    public class ApiSummaryDto
    {
        public int TotalCalls { get; set; }
        public int ErrorCount { get; set; }
        public double AverageDurationMs { get; set; }
    }
}