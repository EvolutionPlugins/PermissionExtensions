# Permission Extensions
Add support prefix, suffix, and color for OpenMod/Unturned!

[![Nuget](https://img.shields.io/nuget/v/DiFFoZ.PermissionExtensions)](https://www.nuget.org/packages/DiFFoZ.PermissionExtensions/)
[![Nuget](https://img.shields.io/nuget/dt/DiFFoZ.PermissionExtensions)](https://www.nuget.org/packages/DiFFoZ.PermissionExtensions/)
[![Discord](https://img.shields.io/discord/764502843906064434?label=Discord%20chat)](https://discord.gg/5MT2yke)

## How install/update plugin
Run command `openmod install DiFFoZ.PermissionExtensions`. After installing reload openmod or restart the server.

# How change prefix/suffix or color
Open the file `openmod.roles.yaml`. You would see new data appeared in all roles (`color`, `prefix`, and `suffix`).

- **Color:** you can see about it [here](https://github.com/DiFFoZ/PermissionExtensions#all-available-colors). Example:
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
- id: policeBase
  priority: 0
  parents:
  - default
  permissions:
  - CuffPlugin:commands.cuff
  - Kits:kits.policeWeapon
  - Kits:kits.policeClothes
  displayName: PoliceBase
  data:
    color: "#5061cf"
    prefix: "[Police] "
    suffix: ""
  isAutoAssigned: false
- id: police
  priority: 5
  parents:
  - policeBase
  permissions:
  - !Kits:kits.policeClothes
  - Kits:kits.policeClothesIV
  - Kits:kits.policeIV
  displayName: Major
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

# Support
You can get support in my discord server [![Discord](https://img.shields.io/discord/764502843906064434?label=Discord%20chat)](https://discord.gg/5MT2yke).
