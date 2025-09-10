using Volo.Abp.Modularity;

namespace MundiFavs;

[DependsOn(
    typeof(MundiFavsDomainModule),
    typeof(MundiFavsTestBaseModule)
)]
public class MundiFavsDomainTestModule : AbpModule
{

}
