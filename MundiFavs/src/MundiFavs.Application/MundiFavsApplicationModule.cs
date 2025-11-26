using Microsoft.Extensions.DependencyInjection;
using MundiFavs.CitySearch;
using MundiFavs.External.CitySearch;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace MundiFavs;

[DependsOn(
    typeof(MundiFavsDomainModule),
    typeof(MundiFavsApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class MundiFavsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MundiFavsApplicationModule>();
        });

        //Registro de GeoDbCitySearchService como implementación de ICitySearchService
        context.Services.AddTransient<ICitySearchService, GeoDbCitySearchService>();
    }
}
