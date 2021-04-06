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
        public async Task HandleEventAsync(object? sender, UnturnedUserConnectingEvent @event)
        {
            var role = await m_PermissionExtensions.GetOrderedPermissionRoleData(@event.User.Id, @event.User.Type);
            if (role == null)
            {
                m_Logger.LogDebug("Role for player {FullName} not found", @event.User.FullActorName);
                return;
            }

            m_Logger.LogDebug("Found role {RoleDisplayName}({RoleId}) for player {FullName}",
                role.DisplayName, role.Id, @event.User.FullActorName);

            var prefix = role.Data?.ContainsKey("prefix") ?? false ? role.Data["prefix"] : string.Empty;
            var suffix = role.Data?.ContainsKey("suffix") ?? false ? role.Data["suffix"] : string.Empty;

            string pendingName = prefix + @event.User.DisplayName + suffix;

            m_Logger.LogDebug("Change name {DisplayName} to {PendingName}", @event.User.DisplayName, pendingName);
            @event.User.SteamPending.playerID.characterName = pendingName;
        }
    }
}
