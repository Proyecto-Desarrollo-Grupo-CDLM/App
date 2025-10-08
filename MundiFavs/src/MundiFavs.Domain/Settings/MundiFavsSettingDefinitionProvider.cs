using Volo.Abp.Settings;

namespace MundiFavs.Settings;

public class MundiFavsSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MundiFavsSettings.MySetting1));
    }
}
