using Microsoft.Extensions.DependencyInjection;
using MundiFavs.CitySearch;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.Modularity;
using MundiFavs;

namespace MundiFavs;

[DependsOn(
    typeof(MundiFavsApplicationModule),
    typeof(MundiFavsDomainTestModule)


)]
public class MundiFavsApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Siempre usar un mock para ICitySearchService en los tests
        var citySearchServiceMock = Substitute.For<ICitySearchService>();
        context.Services.AddSingleton(citySearchServiceMock);
    }
}
