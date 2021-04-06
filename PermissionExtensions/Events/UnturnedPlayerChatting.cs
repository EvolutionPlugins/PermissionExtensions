using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Core.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Chat.Events;
using OpenMod.Unturned.RocketMod;
using SDG.Unturned;
using System.Drawing;
using System.Threading.Tasks;

namespace PermissionExtensions.Events
{
    public class UnturnedPlayerChatting : IEventListener<UnturnedPlayerChattingEvent>
    {
        private readonly PermissionExtensions m_PermissionExtensions;
        private readonly IConfiguration m_Configuration;
        private readonly ILogger<PermissionExtensions> m_Logger;

        public UnturnedPlayerChatting(PermissionExtensions permissionExtensions, IConfiguration configuration,
            ILogger<PermissionExtensions> logger)
        {
            m_PermissionExtensions = permissionExtensions;
            m_Configuration = configuration;
            m_Logger = logger;
        }

        [EventListener(Priority = EventListenerPriority.Normal)]
        public async Task HandleEventAsync(object? sender, UnturnedPlayerChattingEvent @event)
        {
            var isAdmin = @event.Player.SteamPlayer.isAdmin;
            var isGold = @event.Player.SteamPlayer.isPro;
            
            if (!m_Configuration.GetSection("override:color:admin").Get<bool>() && isAdmin || Provider.hideAdmins)
            {
                return;
            }

            if (!m_Configuration.GetSection("override:color:gold").Get<bool>() && isGold)
            {
                return;
            }

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

            unparsedColor ??= "<unknown>";
            m_Logger.LogDebug("Cannot translate color {UnparsedColor} to System.Drawing.Color", unparsedColor);

            var eventIsCancelled = @event.IsCancelled;
            var eventColor = @event.Color.ToSystemColor();
            
            CallRocketPlayerChatted(@event.Player, @event.Mode, @event.Message, ref eventColor,
                ref eventIsCancelled);
            
            @event.IsCancelled = eventIsCancelled;
            @event.Color = eventColor.ToUnityColor();
        }

        private void CallRocketPlayerChatted(UnturnedPlayer player, EChatMode mode, string message, ref Color color,
            ref bool cancel)
        {
            if (!RocketModIntegration.IsRocketModUnturnedLoaded(out _))
            {
                return;
            }

            m_Logger.LogDebug("Calling the event UnturnedChat.OnPlayerChatted");

            var colorEx = color;
            var cancelEx = cancel;

            m_PermissionExtensions.CallRocketEvent(player, mode, message, ref colorEx, ref cancelEx);
            if (m_Configuration.GetSection("rocketmodIntegration:canOverrideColor").Get<bool>())
            {
                color = colorEx;
            }

            if (cancelEx)
            {
                m_Logger.LogDebug("RocketMod cancel the message!");
            }
            if (color != colorEx)
            {
                m_Logger.LogDebug("RocketMod override the color! {FromColor} -> {ToColor}", color, colorEx);
            }
                
            cancel = cancelEx && m_Configuration.GetSection("rocketmodIntegration:canCancelMessage").Get<bool>();

        }
    }
}
