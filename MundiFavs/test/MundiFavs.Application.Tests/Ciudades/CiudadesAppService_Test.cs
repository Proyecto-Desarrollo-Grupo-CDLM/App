using MundiFavs.CitySearch;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;
using NSubstitute; // Usamos NSubstitute para mocking

namespace MundiFavs.Application.CitySearch.Tests
{
    // Clase abstracta base de tests (según el patrón de ABP)
    public class CiudadesAppService_Tests: MundiFavsApplicationTestBase<MundiFavsApplicationTestModule>
        
    {
        private readonly CiudadAppService _ciudadAppService;
        private readonly ICitySearchService _citySearchServiceMock;

        // El constructor prepara el servicio bajo prueba inyectándole el mock
        public CiudadesAppService_Tests()
        {
            // Crear el Mock para la dependencia
            _citySearchServiceMock = Substitute.For<ICitySearchService>();

            // Crear la instancia del AppService, inyectándole el Mock
            _ciudadAppService = new CiudadAppService(_citySearchServiceMock);
        }

        // --- 1. Búsqueda con resultados [cite: 118] ---
        [Fact]
        public async Task SearchCitiesByNameAsync_ReturnsResults()
        {
            // Arrange
            var request = new CitySearchRequestDto { NombreCiudad = "Test" };
            var expected = new CitySearchResultDto
            {
                CityNames = new List<CiudadDto> { new CiudadDto { NombreCiudad = "TestCity", Pais = "TestCountry", Region = "TestRegion", Id = "1" } }
            };

            // Configurar el Mock: cuando se llame a SearchCitiesAsync, debe devolver 'expected'
            _citySearchServiceMock.SearchCitiesAsync(request).Returns(expected);

            // Act
            var result = await _ciudadAppService.SearchCitiesByNameAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.CityNames.Count.ShouldBe(1);
            result.CityNames[0].NombreCiudad.ShouldBe("TestCity");

            // Verificar que el mock fue llamado
            await _citySearchServiceMock.Received(1).SearchCitiesAsync(request);
        }

        // --- 2. Búsqueda sin resultados [cite: 120] ---
        [Fact]
        public async Task SearchCitiesByNameAsync_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { NombreCiudad = "NoMatch" };
            var expected = new CitySearchResultDto { CityNames = new List<CiudadDto>() };

            _citySearchServiceMock.SearchCitiesAsync(request).Returns(expected);

            // Act
            var result = await _ciudadAppService.SearchCitiesByNameAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.CityNames.ShouldBeEmpty();

            await _citySearchServiceMock.Received(1).SearchCitiesAsync(request);
        }

        // --- 3. Entrada inválida (Validación en el AppService) [cite: 122] ---
        [Fact]
        public async Task SearchCitiesByNameAsync_InvalidInput_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { NombreCiudad = "" };

            // Act
            // El AppService tiene la validación interna que debería retornar una lista vacía
            var result = await _ciudadAppService.SearchCitiesByNameAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.CityNames.ShouldBeEmpty();

            // Muy importante: Verificar que el servicio externo NUNCA FUE LLAMADO, 
            // ya que la validación se hizo antes.
            await _citySearchServiceMock.DidNotReceive().SearchCitiesAsync(Arg.Any<CitySearchRequestDto>());
        }

        // --- 4. Error simulado de la API [cite: 124] ---
        [Fact]
        public async Task SearchCitiesByNameAsync_ApiError_ThrowsException()
        {
            // Arrange
            var request = new CitySearchRequestDto { NombreCiudad = "Test" };

            // Configurar el mock para que lance una excepción al ser llamado
            _citySearchServiceMock
                .When(x => x.SearchCitiesAsync(request))
                .Do(x => { throw new Exception("API error: GeoDB unreachable."); });

            // Act & Assert
            // Verificamos que el AppService propaga la excepción del servicio externo.
            await Assert.ThrowsAsync<Exception>(() => _ciudadAppService.SearchCitiesByNameAsync(request));

            await _citySearchServiceMock.Received(1).SearchCitiesAsync(request);
        }
    }
}