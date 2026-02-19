using MundiFavs.Localization;
using Volo.Abp.Application.Services;

namespace MundiFavs;

/* Inherit your application services from this class.
 */
public abstract class MundiFavsAppService : ApplicationService
{
    protected MundiFavsAppService()
    {
        LocalizationResource = typeof(MundiFavsResource);
    }
}
