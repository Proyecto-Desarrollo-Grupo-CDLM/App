using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace MundiFavs.ApiMetrics
{
    public class ApiMetricManager : DomainService
    {
        private readonly IRepository<ApiMetric, Guid> _metricRepository;

        public ApiMetricManager(IRepository<ApiMetric, Guid> metricRepository)
        {
            _metricRepository = metricRepository;
        }

        public async Task LogMetricAsync(string apiName, string endpoint, int duration, int statusCode, bool isSuccess)
        {
            var metric = new ApiMetric(GuidGenerator.Create(), apiName, endpoint, duration, statusCode, isSuccess);
            await _metricRepository.InsertAsync(metric);
        }
    }
}
