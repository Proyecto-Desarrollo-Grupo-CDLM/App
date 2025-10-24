using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MundiFavs.External.CitySearch;
using MundiFavs.CitySearch;
using MundiFavs.External;

namespace MundiFavs.Application.Tests.External
{
    public class GeoDbCitySearchService_IntegrationTests
    {
        private class FailingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network error");
            }
        }

        private GeoDbCitySearchService CreateService()
        {
            var httpClient = new HttpClient();
            return new GeoDbCitySearchService(httpClient);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsResults_ForValidNombreCiudad()
        {
            var service = CreateService();
            var request = new CitySearchRequestDto { NombreCiudad = "Paris" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.NotEmpty(result.CityNames);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsEmpty_ForNoMatch()
        {
            var service = CreateService();
            var request = new CitySearchRequestDto { NombreCiudad = "zzzzzzzzzz" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.CityNames);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_ReturnsEmpty_ForInvalidInput()
        {
            var service = CreateService();
            var request = new CitySearchRequestDto { NombreCiudad = "" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.CityNames);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_HandlesNetworkError()
        {
            // Simula error de red usando un HttpClient con un handler que lanza excepción
            var httpClient = new HttpClient(new FailingHandler());
            var service = new GeoDbCitySearchService(httpClient);
            var request = new CitySearchRequestDto { NombreCiudad = "Rio" };
            var result = await service.SearchCitiesAsync(request);
            Assert.NotNull(result);
            Assert.Empty(result.CityNames);
        }
    }
}