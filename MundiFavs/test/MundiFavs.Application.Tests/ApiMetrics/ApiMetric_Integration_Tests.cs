using MundiFavs.ApiMetrics;
using MundiFavs.CitySearch;
using MundiFavs.Destinos;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Xunit;

namespace MundiFavs.External
{
    public class ApiMetric_Integration_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICitySearchService _citySearchService;
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ApiMetric_Integration_Tests()
        {
            // Aquí ABP inyectará el Decorador porque lo registramos como ICitySearchService
            _citySearchService = GetRequiredService<ICitySearchService>();
            _apiMetricRepository = GetRequiredService<IRepository<ApiMetric, Guid>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Save_Metric_When_Searching_Cities()
        {
            try
            {
                await _citySearchService.SearchCitiesAsync(new CitySearchRequestDto { NombreCiudad = "Buenos Aires" });
            }
            catch { /* ignoramos errores de API externa */ }

            // Assert
            // Usamos el repositorio directamente
            var metrics = await _apiMetricRepository.GetListAsync();

            // TIP DE DEBUG: Si esto falla, poné un breakpoint aquí y mirá qué tiene la variable 'metrics'
            metrics.ShouldNotBeEmpty();
            metrics.Any(x => x.Endpoint == "SearchCitiesAsync").ShouldBeTrue();
        }



        [Fact]
        public async Task Should_Delete_Old_Metrics_Only()
        {
            var apiMetricRepository = GetRequiredService<IRepository<ApiMetric, Guid>>();
            var apiMetricAppService = GetRequiredService<IApiMetricAppService>();

            // Arrange: Insertar una métrica vieja (manual al repo porque el manager pone fecha actual)
            var oldId = Guid.NewGuid();
            var oldMetric = new ApiMetric(oldId, "Old API", "Old", 100, 200, true);
            // Usamos reflexión o un método de test para setear la fecha de hace 40 días
            typeof(ApiMetric).GetProperty(nameof(ApiMetric.ExecutionTime))?
                .SetValue(oldMetric, DateTime.Now.AddDays(-40));

            await apiMetricRepository.InsertAsync(oldMetric);

            // Act
            await apiMetricAppService.DeleteOldMetricsAsync();

            // Assert
            var deletedMetric = await apiMetricRepository.FindAsync(oldId);
            deletedMetric.ShouldBeNull();
        }
    }
}