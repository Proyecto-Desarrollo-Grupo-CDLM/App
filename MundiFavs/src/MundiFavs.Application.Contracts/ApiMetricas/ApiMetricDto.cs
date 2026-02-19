using System;
using Volo.Abp.Application.Dtos;

namespace MundiFavs.ApiMetrics
{
    public class ApiMetricDto : EntityDto<Guid>
    {
        public string ApiName { get; set; }
        public string Endpoint { get; set; }
        public int DurationMs { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
}