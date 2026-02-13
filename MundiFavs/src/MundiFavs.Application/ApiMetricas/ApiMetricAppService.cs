using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace MundiFavs.ApiMetrics
{

    [Authorize]

    public class ApiMetricAppService : MundiFavsAppService, IApiMetricAppService
    {
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;

        public ApiMetricAppService(IRepository<ApiMetric, Guid> apiMetricRepository)
        {
            _apiMetricRepository = apiMetricRepository;
        }
        
        public async Task<List<ApiMetricDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();

            var metrics = await AsyncExecuter.ToListAsync(
                queryable.OrderByDescending(x => x.ExecutionTime)
                        .Take(50)
            );

            return ObjectMapper.Map<List<ApiMetric>, List<ApiMetricDto>>(metrics);

        }

        public async Task<ApiSummaryDto> GetSummaryAsync()
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();
            var allMetrics = await AsyncExecuter.ToListAsync(queryable);


            return new ApiSummaryDto
            {
                TotalCalls = allMetrics.Count,
                ErrorCount = allMetrics.Count(x => !x.IsSuccess),
                AverageDurationMs = allMetrics.Any() ? allMetrics.Average(x => x.DurationMs) : 0
            };
        }


        public async Task<List<ApiMetricDto>> GetFilteredListAsync(
            int? statusCode,
            string endpoint,
            DateTime? fromDate)
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();

            // Aplicamos filtros condicionales
            queryable = queryable
                .WhereIf(statusCode.HasValue, x => x.StatusCode == statusCode)
                .WhereIf(!string.IsNullOrWhiteSpace(endpoint), x => x.Endpoint.Contains(endpoint))
                .WhereIf(fromDate.HasValue, x => x.ExecutionTime >= fromDate);

            var metrics = await AsyncExecuter.ToListAsync(
                queryable.OrderByDescending(x => x.ExecutionTime)
                         .Take(100) // Limitamos a 100 para evitar lentitud
            );

            return ObjectMapper.Map<List<ApiMetric>, List<ApiMetricDto>>(metrics);
        }


        //Metodo para eliminar métricas antiguas (más de 30 días)

        public async Task DeleteOldMetricsAsync()
        {
            
            var dateLimit = Clock.Now.AddDays(-30);

           
            var queryable = await _apiMetricRepository.GetQueryableAsync();

            
            var oldMetrics = queryable.Where(x => x.ExecutionTime < dateLimit).ToList();

            if (oldMetrics.Any())
            {
                foreach (var metric in oldMetrics)
                {
                    await _apiMetricRepository.DeleteAsync(metric);
                }

                Logger.LogInformation($"Se han eliminado {oldMetrics.Count} métricas antiguas (anteriores a {dateLimit}).");
            }
        }
    }
}