using MundiFavs.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MundiFavs.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MundiFavsEntityFrameworkCoreModule),
    typeof(MundiFavsApplicationContractsModule)
)]
public class MundiFavsDbMigratorModule : AbpModule
{
}
