using Volo.Abp.Modularity;

namespace MundiFavs;

public abstract class MundiFavsApplicationTestBase<TStartupModule> : MundiFavsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
