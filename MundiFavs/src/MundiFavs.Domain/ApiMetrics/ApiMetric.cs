using System;
using Volo.Abp.Domain.Entities;

namespace MundiFavs.ApiMetrics
{
    public class ApiMetric : Entity<Guid>
    {
        public string ApiName { get; set; }        // Ej: "GeoDB Cities"
        public string Endpoint { get; set; }       // Ej: "SearchCities"
        public int DurationMs { get; set; }        // Tiempo de respuesta
        public int StatusCode { get; set; }        // 200, 404, 500, etc.
        public bool IsSuccess { get; set; }
        public DateTime ExecutionTime { get; set; }

        protected ApiMetric() { }

        public ApiMetric(Guid id, string apiName, string endpoint, int durationMs, int statusCode, bool isSuccess)
            : base(id)
        {
            ApiName = apiName;
            Endpoint = endpoint;
            DurationMs = durationMs;
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ExecutionTime = DateTime.Now;
        }
    }
}