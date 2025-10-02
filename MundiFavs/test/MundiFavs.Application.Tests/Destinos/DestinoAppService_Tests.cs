
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
// using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MundiFavs.Destinos;

public abstract class DestinoAppService_Tests<TStartupModule> : MundiFavsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDestinoAppService _destinoAppService;

    protected DestinoAppService_Tests()
    {
        _destinoAppService = GetRequiredService<IDestinoAppService>();
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
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _destinoAppService.CreateAsync(
                new CreateUpdateDestinoDto
                {
                    Nombre = "",
                    Pais = "Francia",
                    Ciudad = "Paris",
                    Poblacion = 2150000,
                    Latitud = 48.84M,
                    Longitud = 2.34M,
                    ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/AC9h4nqRKD6Hgtx5_i_49neoPWQadN13YEerMr2ATVmyt1hJvfnGPG91MNIynqowDyjOrNZ2gk5gJ4JtpZBl5VAZRB-Gd_d4ZT1C595MBYvDe9ElsWZSTN5g6cVdXcSzq2Whwr8VQweOb5aIjbA=s1360-w1360-h1020-rw"

                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "Nombre"));
    }



}
