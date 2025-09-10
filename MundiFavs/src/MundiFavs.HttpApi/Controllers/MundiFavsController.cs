using MundiFavs.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MundiFavs.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MundiFavsController : AbpControllerBase
{
    protected MundiFavsController()
    {
        LocalizationResource = typeof(MundiFavsResource);
    }
}
