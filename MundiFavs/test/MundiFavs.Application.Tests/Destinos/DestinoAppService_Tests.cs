
//using Autofac.Core;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MundiFavs.CitySearch;
using MundiFavs.Destinos;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
//using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;
//using static OpenIddict.Abstractions.OpenIddictConstants;

// using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MundiFavs.Application.Tests.Destinos;

public abstract class DestinoAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDestinoAppService _destinoAppService;

    private readonly IDestinoAppService _service;
    private readonly ICitySearchService _citySearchService;
    private readonly IRepository<Destino, Guid> _repository;
    //private readonly IDbContextProvider<MundiFavsDbContext> _dbContextProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    protected DestinoAppService_Tests()
    {
        _destinoAppService = GetRequiredService<IDestinoAppService>();
        _service = ServiceProvider.GetRequiredService<IDestinoAppService>();
        {

            //_dbContextProvider = GetRequiredService<IDbContextProvider<MundiFavsDbContext>>();
            _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();

            _repository = GetRequiredService<IRepository<Destino, Guid>>();
            _citySearchService = Substitute.For<ICitySearchService>();
            _service = GetRequiredService<IDestinoAppService>();
        }
    }

    [Fact]
    public async Task Should_Get_List_Of_Destinos()
    {
        //Act
        var result = await _destinoAppService.GetListAsync(
           new PagedAndSortedResultRequestDto()
        );

        //Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThan(0);
        //   result.Items.ShouldContain(b => b.Nombre == "Casa de Urquiza");
    }





    [Fact]
    public async Task Should_Create_A_Valid_Destino()
    {   //Arrange
        var input = new CreateUpdateDestinoDto
        {
            Nombre = "Torre Eiffel",
            Pais = "Francia",
            Ciudad = "Paris",
            Poblacion = 2150000,
            Latitud = 48.84M,
            Longitud = 2.34M,
            ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/AC9h4nqRKD6Hgtx5_i_49neoPWQadN13YEerMr2ATVmyt1hJvfnGPG91MNIynqowDyjOrNZ2gk5gJ4JtpZBl5VAZRB-Gd_d4ZT1C595MBYvDe9ElsWZSTN5g6cVdXcSzq2Whwr8VQweOb5aIjbA=s1360-w1360-h1020-rw"
        };
        //Act
        var result = await _destinoAppService.CreateAsync(input);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Nombre.ShouldBe(input.Nombre);
        result.Pais.ShouldBe(input.Pais);
        result.Ciudad.ShouldBe(input.Ciudad);
        result.Poblacion.ShouldBe(input.Poblacion);
        result.Ubicacion.Latitud.ShouldBe(input.Latitud);
        result.Ubicacion.Longitud.ShouldBe(input.Longitud);
        //result.ImageUrl.ShouldBe(input.ImageUrl);  //no validamos URL
    }

    [Fact]
    public async Task Should_Not_Create_A_Destino_Without_Name()
    {
        // Arrange
        var input = new CreateUpdateDestinoDto
        {
            Nombre ="", // probamos campo vacío
            Pais = "Francia",
            Ciudad = "Paris",
            Poblacion = 2150000,
            Latitud = 48.84M,
            Longitud = 2.34M,
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var exception = await Assert.ThrowsAsync<AbpValidationException>(
            () => _destinoAppService.CreateAsync(input)
        );

        // Assert
        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == nameof(input.Nombre)));
    }

    [Fact]
    public async Task CreateASync_ShouldPersistDestinationInDataBase()
    { //arrange
        var input = new CreateUpdateDestinoDto
        {
            Nombre = "Museo del Louvre",
            Pais = "Francia",
            Ciudad = "Paris",
            Poblacion = 2150000,
            Latitud = 48.84M,
            Longitud = 2.34M,
            ImageUrl = "https://example.com/image.jpg"


        };
    //act
    var createdDestination = await _destinoAppService.CreateAsync(input);
        var retrievedDestination = await _destinoAppService.GetAsync(createdDestination.Id);

        //assert
        retrievedDestination.ShouldNotBeNull();
    retrievedDestination.Id.ShouldBe(createdDestination.Id);
    retrievedDestination.Nombre.ShouldBe(input.Nombre);
    retrievedDestination.Pais.ShouldBe(input.Pais);
    retrievedDestination.Ciudad.ShouldBe(input.Ciudad);
    retrievedDestination.Poblacion.ShouldBe(input.Poblacion);
    retrievedDestination.Ubicacion.Latitud.ShouldBe(input.Latitud);
    retrievedDestination.Ubicacion.Longitud.ShouldBe(input.Longitud);
    //retrievedDestination.ImageUrl.ShouldBe(input.ImageUrl); //no validamos URL

  

    }

    [Fact]
    public async Task SearchCitiesAsync_ReturnsResults()
    {
        // Arrange
        var request = new CitySearchRequestDto { NombreCiudad = "Test" };
        var expected = new CitySearchResultDto
        {
            CityNames = new List<CiudadDto> { new CiudadDto { NombreCiudad = "TestCity", Pais = "TestCountry", Region = "TestRegion", Id = "1" } }
        };
        var repoMock = Substitute.For<IRepository<Destino, Guid>>();
        var citySearchMock = Substitute.For<ICitySearchService>();
        citySearchMock.SearchCitiesAsync(request).Returns(expected);
        var service = new DestinoAppService(repoMock, citySearchMock);

        // Act
        var result = await service.SearchCitiesAsync(request);

        // Assert
        result.ShouldNotBeNull();
        result.CityNames.Count.ShouldBe(1);
        result.CityNames[0].NombreCiudad.ShouldBe("TestCity");
    }

    [Fact]
    public async Task SearchCitiesAsync_ReturnsEmpty()
    {
        // Arrange
        var request = new CitySearchRequestDto { NombreCiudad  = "NoMatch" };
        var expected = new CitySearchResultDto { CityNames = new List<CiudadDto>() };
        var repoMock = Substitute.For<IRepository<Destino, Guid>>();
        var citySearchMock = Substitute.For<ICitySearchService>();
        citySearchMock.SearchCitiesAsync(request).Returns(expected);
        var service = new DestinoAppService(repoMock, citySearchMock);

        // Act
        var result = await service.SearchCitiesAsync(request);

        // Assert
        result.ShouldNotBeNull();
        result.CityNames.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchCitiesAsync_InvalidInput_ReturnsEmpty()
    {
        // Arrange
        var request = new CitySearchRequestDto { NombreCiudad = "" };
        var expected = new CitySearchResultDto { CityNames = new List<CiudadDto>() };
        var repoMock = Substitute.For<IRepository<Destino, Guid>>();
        var citySearchMock = Substitute.For<ICitySearchService>();
        citySearchMock.SearchCitiesAsync(request).Returns(expected);
        var service = new DestinoAppService(repoMock, citySearchMock);

        // Act
        var result = await service.SearchCitiesAsync(request);

        // Assert
        result.ShouldNotBeNull();
        result.CityNames.ShouldBeEmpty();
    }

    [Fact]
    public async Task SearchCitiesAsync_ApiError_ThrowsException()
    {
        // Arrange
        var request = new CitySearchRequestDto { NombreCiudad = "Test" };
        var repoMock = Substitute.For<IRepository<Destino, Guid>>();
        var citySearchMock = Substitute.For<ICitySearchService>();
        citySearchMock
            .When(x => x.SearchCitiesAsync(request))
            .Do(x => { throw new Exception("API error"); });
        var service = new DestinoAppService(repoMock, citySearchMock);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.SearchCitiesAsync(request));
    }

}
