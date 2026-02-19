using System.Threading.Tasks;

namespace MundiFavs.Data;

public interface IMundiFavsDbSchemaMigrator
{
    Task MigrateAsync();
}
