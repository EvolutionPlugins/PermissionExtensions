using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

[assembly: PluginMetadata("PermissionExtensions", Author = "DiFFoZ", DisplayName = "Permission Extensions",
    Website = "https://github.com/DiFFoZ/PermissionExtensions")]

namespace PermissionExtensions
{
    public class PermissionExtensions : OpenModUnturnedPlugin
    {
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;
        private readonly IUserDataStore m_UserDataStore;

        public PermissionExtensions(IServiceProvider serviceProvider, IPermissionRolesDataStore permissionRolesDataStore,
            IUserDataStore userDataStore) : base(serviceProvider)
        {
            m_PermissionRolesDataStore = permissionRolesDataStore;
            m_UserDataStore = userDataStore;
        }

        protected override UniTask OnLoadAsync()
        {
            Provider.onCheckValidWithExplanation += OnCheckValidWithExplanation;
            ChatManager.onChatted += OnChatted;
            return AddExample().AsUniTask(false);
        }

        protected override UniTask OnUnloadAsync()
        {
            Provider.onCheckValidWithExplanation -= OnCheckValidWithExplanation;
            ChatManager.onChatted -= OnChatted;
            return UniTask.CompletedTask;
        }

        private void OnChatted(SteamPlayer player, EChatMode mode, ref UnityEngine.Color chatted, ref bool isRich,
            string text, ref bool isVisible)
        {
            if (!isVisible) return;
            var altColor = chatted;

            AsyncHelper.RunSync(async () =>
            {
                var role = await GetOrderedPermissionRoleData(player.playerID.steamID.ToString());
                if (role?.Data.ContainsKey("color") == true)
                {
                    var color = ColorTranslator.FromHtml(role.Data["color"].ToString());
                    if (!color.IsEmpty)
                    {
                        altColor = color.ToUnityColor();
                    }
                }
            });

            chatted = altColor;
        }

        private void OnCheckValidWithExplanation(ValidateAuthTicketResponse_t callback, ref bool isValid,
            ref string explanation)
        {
            if (!isValid) return;

            var steamId = callback.m_SteamID;
            var pending = Provider.pending.Find(c => c.playerID.steamID == steamId);

            if (pending != null)
            {
                AsyncHelper.RunSync(async () =>
                {
                    var role = await GetOrderedPermissionRoleData(steamId.ToString());

                    if (role != null)
                    {
                        var suffix = role.Data.ContainsKey("suffix") ? role.Data["suffix"] : string.Empty;
                        var prefix = role.Data.ContainsKey("prefix") ? role.Data["prefix"] : string.Empty;

                        pending.playerID.characterName = prefix + pending.playerID.characterName + suffix;
                    }
                });
            }
        }

        private async Task<PermissionRoleData> GetOrderedPermissionRoleData(string id)
        {
            PermissionRoleData result = null;

            var data = await m_UserDataStore.GetUserDataAsync(id, KnownActorTypes.Player);
            if (data != null)
            {
                foreach (var roleId in data.Roles)
                {
                    var role = await m_PermissionRolesDataStore.GetRoleAsync(roleId);
                    if (role == null)
                    {
                        continue;
                    }

                    if ((result?.Priority ?? -1) < role.Priority)
                    {
                        result = role;
                    }
                }
            }

            return result;
        }

        private Task AddExample()
        {
            if (!Configuration.GetSection("addExample").Get<bool>())
            {
                return Task.CompletedTask;
            }

            foreach (var role in m_PermissionRolesDataStore.Roles)
            {
                if (!role.Data.ContainsKey("color"))
                {
                    role.Data.Add("color", ColorTranslator.ToHtml(Color.White));
                }
                if (!role.Data.ContainsKey("prefix"))
                {
                    role.Data.Add("prefix", string.Empty);
                }
                if (!role.Data.ContainsKey("suffix"))
                {
                    role.Data.Add("suffix", string.Empty);
                }
            }
            return m_PermissionRolesDataStore.SaveChangesAsync();
        }
    }
}
