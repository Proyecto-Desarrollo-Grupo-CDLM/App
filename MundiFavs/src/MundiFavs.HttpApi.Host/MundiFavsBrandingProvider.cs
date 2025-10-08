using Microsoft.Extensions.Localization;
using MundiFavs.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MundiFavs;

[Dependency(ReplaceServices = true)]
public class MundiFavsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MundiFavsResource> _localizer;

    public MundiFavsBrandingProvider(IStringLocalizer<MundiFavsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
