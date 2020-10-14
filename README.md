# Permission Extensions
Add support prefix, suffix, and color for OpenMod/Unturned!

[![Nuget](https://img.shields.io/nuget/v/DiFFoZ.PermissionExtensions)](https://www.nuget.org/packages/DiFFoZ.PermissionExtensions/)
[![Nuget](https://img.shields.io/nuget/dt/DiFFoZ.PermissionExtensions)](https://www.nuget.org/packages/DiFFoZ.PermissionExtensions/)
[![Discord](https://img.shields.io/discord/764502843906064434?label=Discord%20chat)](https://discord.gg/5MT2yke)

## How install/update plugin
Run command `openmod install DiFFoZ.PermissionExtensions`. After installing reload openmod or restart the server.

# How change prefix/suffix or color
First at all [install a plugin](https://github.com/DiFFoZ/PermissionExtensions#how-installupdate-plugin) then open file `openmod.roles.yaml`. You will see a new data appears in all roles (`color`, `prefix`, and `suffix`).

- **Color:** you can see about it [there](https://github.com/DiFFoZ/PermissionExtensions#all-available-colors). Example:
```yaml
color: Blue
```
- **Prefix:** adds a prefix to the nickname at the start. Example: 
```yaml
prefix: "[Police] "
```
- **Suffix:** adds a suffix to the nickname at the end. Example:
```yaml
suffix: " [IV]"
```
Example how will be look `openmod.roles.yaml`:
```yaml
roles:
- id: vip
  priority: 1
  parents:
  - default
  permissions:
  - Kits:kits.vip
  displayName: 
  data:
    color: Blue
    prefix: "[Police] "
    suffix: " [IV]"
  isAutoAssigned: false
  
...
```
**At the end nickname will be `[Police] %PlayerName% [IV]`**.

# All available colors
![Colors](https://docs.microsoft.com/en-us/dotnet/media/art-color-table.png?view=netcore-3.1)

Also, you can use hex colors. Just set the color to e.g. 
```yaml
color: "#2ed264"
```

You can read more detailed about it in [docs](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.colors?view=netcore-3.1).
