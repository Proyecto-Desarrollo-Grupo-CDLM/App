using Volo.Abp.Modularity;

namespace MundiFavs;

[DependsOn(
    typeof(MundiFavsApplicationModule),
    typeof(MundiFavsDomainTestModule)
)]
public class MundiFavsApplicationTestModule : AbpModule
{

}
