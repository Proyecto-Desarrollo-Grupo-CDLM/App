using Volo.Abp.Modularity;

namespace MundiFavs;

/* Inherit from this class for your domain layer tests. */
public abstract class MundiFavsDomainTestBase<TStartupModule> : MundiFavsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
