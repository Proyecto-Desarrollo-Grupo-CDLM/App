using MundiFavs.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MundiFavs.Permissions;

public class MundiFavsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MundiFavsPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MundiFavsPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MundiFavsResource>(name);
    }
}
