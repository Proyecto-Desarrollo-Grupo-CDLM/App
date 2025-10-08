using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MundiFavs.Data;

/* This is used if database provider does't define
 * IMundiFavsDbSchemaMigrator implementation.
 */
public class NullMundiFavsDbSchemaMigrator : IMundiFavsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
