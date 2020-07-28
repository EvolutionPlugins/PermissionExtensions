using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("PermissionExtensions", Author = "DiFFoZ", DisplayName = "Permission Extensions")]

namespace PermissionExtensions
{
    public class PermissionExtensions : OpenModUnturnedPlugin
    {
        public PermissionExtensions(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
