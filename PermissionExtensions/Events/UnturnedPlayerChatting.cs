using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Players.Chat.Events;
using OpenMod.Unturned.RocketMod;
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
        public async Task HandleEventAsync(object? sender, UnturnedPlayerChattingEvent @event)
        {
            var id = @event.Player.SteamId.ToString();
            var displayName = @event.Player.SteamPlayer.playerID.characterName;

            var role = await m_PermissionExtensions.GetOrderedPermissionRoleData(id, KnownActorTypes.Player);
            if (role == null)
            {
                m_Logger.LogDebug("Role for player {Name}({Id}) not found", displayName, id);
                return;
            }

            m_Logger.LogDebug("Found role {RoleDisplayName}({RoleId}) for player {PlayerName}({PlayerId})",
                role.DisplayName, role.Id, displayName, id);

            if (role.Data!.TryGetValue("color", out var unparsedColor) && unparsedColor is string @string)
            {
                var color = ColorTranslator.FromHtml(@string);

                if (!color.IsEmpty)
                {
                    m_Logger.LogDebug("Change color {UColor} to {SColor}", @event.Color, color);
                    @event.Color = color.ToUnityColor();
                    return;
                }
            }

            m_Logger.LogDebug("Cannot translate color {unparsedColor} to System.Drawing.Color", unparsedColor ?? string.Empty);

            if (RocketModIntegration.IsRocketModUnturnedLoaded(out _))
            {
                m_Logger.LogDebug("Calling the event UnturnedChat.OnPlayerChatted");

                var color = @event.Color.ToSystemColor();
                var cancel = @event.IsCancelled;

                m_PermissionExtensions.CallRocketEvent(@event.Player, @event.Mode, @event.Message, ref color, ref cancel);
                @event.Color = color.ToUnityColor();
                @event.IsCancelled = cancel;
            }
        }
    }
}
