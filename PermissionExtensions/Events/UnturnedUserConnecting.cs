using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.Users.Events;
using System.Threading.Tasks;

namespace PermissionExtensions.Events
{
    public class UnturnedUserConnecting : IEventListener<UnturnedUserConnectingEvent>
    {
        private readonly PermissionExtensions m_PermissionExtensions;
        private readonly ILogger<PermissionExtensions> m_Logger;

        public UnturnedUserConnecting(PermissionExtensions permissionExtensions, ILogger<PermissionExtensions> logger)
        {
            m_PermissionExtensions = permissionExtensions;
            m_Logger = logger;
        }

        [EventListener(Priority = EventListenerPriority.Normal)]
        public async Task HandleEventAsync(object sender, UnturnedUserConnectingEvent @event)
        {
            var role = await m_PermissionExtensions.GetOrderedPermissionRoleData(@event.User.Id);
            if (role == null)
            {
                m_Logger.LogTrace("Role not found");
                return;
            }

            var prefix = role.Data.ContainsKey("prefix") ? role.Data["prefix"] : string.Empty;
            var suffix = role.Data.ContainsKey("suffix") ? role.Data["suffix"] : string.Empty;
            string pendingName = prefix + @event.User.DisplayName + suffix;
            m_Logger.LogTrace($"Change name {@event.User.DisplayName} to {pendingName}");
            @event.User.SteamPending.playerID.characterName = pendingName;
        }
    }
}
