using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MundiFavs;
using MundiFavs.CitySearch;
using NSubstitute;
using Volo.Abp;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

using MundiFavs.EntityFrameworkCore;
using Volo.Abp.Data;


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