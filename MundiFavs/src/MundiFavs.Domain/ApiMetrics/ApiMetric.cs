using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace MundiFavs.Domain.ApiMetrics
{
    public class ApiMetric : CreationAuditedAggregateRoot<Guid>
    {
        // Información básica de la llamada
        public string Endpoint { get; private set; }
        public string HttpMethod { get; private set; }
        public string RequestUrl { get; private set; }
        
        // Métricas de rendimiento
        public long ResponseTimeMs { get; private set; }
        public int StatusCode { get; private set; }
        public bool IsSuccess { get; private set; }

        // Información de errores (si hubo alguno)
        public string ErrorMessage { get; private set; }
        public string ErrorType { get; private set; }

        // Información adicional
        public DateTime RequestDateTime { get; private set; }
        public string UserId { get; private set; }
        public int? ResultCount { get; private set; } //? significa que puede ser null
        public string RequestParameters { get; private set; }

        // Constructor privado para Entity Framework
        private ApiMetric()
        {
        }

        // Constructor público - la forma correcta de crear un ApiMetric
        public ApiMetric(
            Guid id,
            string endpoint,
            string httpMethod,
            string requestUrl,
            long responseTimeMs,
            int statusCode,
            bool isSuccess,
            string userId = null,
            string errorMessage = null,
            string errorType = null,
            int? resultCount = null,
            string requestParameters = null
        ) : base(id)
        {
            Endpoint = endpoint;
            HttpMethod = httpMethod;
            RequestUrl = requestUrl;
            ResponseTimeMs = responseTimeMs;
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            UserId = userId;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
            ResultCount = resultCount;
            RequestParameters = requestParameters;
            RequestDateTime = DateTime.UtcNow;
        }

        // Método para crear fácilmente un nuevo ApiMetric
        public static ApiMetric Create(
            string endpoint,
            string httpMethod,
            string requestUrl,
            long responseTimeMs,
            int statusCode,
            bool isSuccess,
            string userId = null,
            string errorMessage = null,
            string errorType = null,
            int? resultCount = null,
            string requestParameters = null
        )
        {
            return new ApiMetric(
                Guid.NewGuid(),  // Genera un ID único automáticamente
                endpoint,
                httpMethod,
                requestUrl,
                responseTimeMs,
                statusCode,
                isSuccess,
                userId,
                errorMessage,
                errorType,
                resultCount,
                requestParameters
            );
        }
    }
}