using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Players.Chat.Events;
using System.Drawing;
using System.Threading.Tasks;

namespace PermissionExtensions.Events
{
    public class UnturnedPlayerChatting : IEventListener<UnturnedPlayerChattingEvent>
    {
        private readonly PermissionExtensions m_PermissionExtensions;
        private readonly ILogger<PermissionExtensions> m_Logger;

        public UnturnedPlayerChatting(PermissionExtensions permissionExtensions, ILogger<PermissionExtensions> logger)
        {
            m_PermissionExtensions = permissionExtensions;
            m_Logger = logger;
        }

        [EventListener(Priority = EventListenerPriority.Normal)]
        public async Task HandleEventAsync(object sender, UnturnedPlayerChattingEvent @event)
        {
            var role = await m_PermissionExtensions.GetOrderedPermissionRoleData(@event.Player.SteamId.ToString());
            if(role == null)
            {
                m_Logger.LogDebug("Role not found");
                return;
            }
            m_Logger.LogDebug($"Role founded: {role.Id}");

            if (role.Data.ContainsKey("color"))
            {
                var colorData = role.Data["color"].ToString();
                var color = ColorTranslator.FromHtml(colorData);
                if (!color.IsEmpty)
                {
                    m_Logger.LogDebug($"Change color {@event.Color} to {color}");
                    @event.Color = color.ToUnityColor();
                    return;
                }
                m_Logger.LogDebug($"Cannot translate color {colorData} to System.Drawing.Color");
            }
        }
    }
}
