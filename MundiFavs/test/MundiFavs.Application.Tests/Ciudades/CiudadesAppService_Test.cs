using MundiFavs.Application.CitySearch;
using MundiFavs.CitySearch;
using NSubstitute; 
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace MundiFavs.Application.Tests.CitySearch
{
    
    public class CiudadesAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule

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
        //-----------------------------------------------------------------------------------
        // Pruebas para la operación 3.3: Obtener detalles de una ciudad
        // -----------------------------------------------------------------------------------

        [Fact]
        public async Task Should_Get_Detail_Successfully()
        {
            // Arrange
            var request = new CityDetailRequestDto { CityId = "Q1192" };

            var expectedDetail = new CityDetailDto
            {
                Id = "Q1192",
                NombreCiudad = "Buenos Aires",
                Pais = "Argentina",
                Poblacion = 3000000,
                Latitud = -34.6037m,
                Longitud = -58.3816m
            };

            // ⚠️ Configurar el Substitute: 
            // Cuando se llame a GetCityDetailById con CUALQUIER CityDetailRequestDto, devuelve expectedDetail.
            _citySearchServiceMock
                .GetCityDetailById(Arg.Any<CityDetailRequestDto>())
                .Returns(Task.FromResult(expectedDetail)); // Usar Task.FromResult para métodos async

            // Act
            var result = await _ciudadAppService.GetCityDetail(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDetail.NombreCiudad, result.NombreCiudad);
            Assert.Equal(expectedDetail.Poblacion, result.Poblacion);

            // ⚠️ Verificar: 
            // Verificar que el método fue llamado una sola vez con el argumento específico 'request'.
            await _citySearchServiceMock
                .Received(1)
                .GetCityDetailById(request);
        }

        // -----------------------------------------------------------------------------------
        // Caso 2: Validación de Entrada Inválida (Verificación con .DidNotReceive)
        // -----------------------------------------------------------------------------------
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Should_Throw_Exception_When_CityId_Is_Null_Or_Empty(string invalidId)
        {
            // Arrange: Petición con ID inválido
            var request = new CityDetailRequestDto { CityId = invalidId };

            // Act & Assert: Esperamos que lance una excepción amigable (UserFriendlyException)
            await Assert.ThrowsAsync<UserFriendlyException>(
                () => _ciudadAppService.GetCityDetail(request)
            );

            // ⚠️ Verificar: El Application Service NO DEBE llamar al servicio externo
            await _citySearchServiceMock
                .DidNotReceive() // Usa DidNotReceive() para verificar que NO FUE llamado
                .GetCityDetailById(Arg.Any<CityDetailRequestDto>());
        }

        // -----------------------------------------------------------------------------------
        // Caso 3: Manejo de Ciudad No Encontrada (Configuración con .Returns(Task.FromException))
        // -----------------------------------------------------------------------------------
        [Fact]
        public async Task Should_Propagate_EntityNotFoundException()
        {
            // Arrange
            var request = new CityDetailRequestDto { CityId = "INVALID_ID_999" };
            var notFoundException = new EntityNotFoundException();

            // ⚠️ Configurar el Substitute: 
            // Simular que el servicio externo lanza la excepción
            _citySearchServiceMock
                .GetCityDetailById(Arg.Any<CityDetailRequestDto>())
                .Returns(Task.FromException<CityDetailDto>(notFoundException)); // Usamos Task.FromException para simular async throw

            // Act & Assert: Esperamos que el AppService propague la misma excepción
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _ciudadAppService.GetCityDetail(request)
            );

            // ⚠️ Verificar: Se verifica que SÍ se intentó la llamada al servicio externo
            await _citySearchServiceMock
                .Received(1)
                .GetCityDetailById(request);
        }
    }
}

