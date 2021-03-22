using HarmonyLib;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Patching;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Drawing;
using System.Reflection;

namespace PermissionExtensions
{
    public class RocketModHandleChatPatch
    {
        private readonly ILogger<PermissionExtensions> m_Logger;
        private readonly Assembly m_Assembly;
        private readonly Harmony m_Harmony;
        private readonly MethodInfo? m_UnturnedPlayerEventsFirePlayerChattedMethod;
        private readonly MethodInfo? m_UnturnedPlayerFromPlayerMethod;

        public RocketModHandleChatPatch(ILogger<PermissionExtensions> logger, Harmony harmony, Assembly assembly)
        {
            m_Logger = logger;
            m_Harmony = harmony;
            m_Assembly = assembly;

            m_UnturnedPlayerEventsFirePlayerChattedMethod = m_Assembly
                .GetType("Rocket.Unturned.Events.UnturnedPlayerEvents", false)?
                .GetMethod("firePlayerChatted", BindingFlags.NonPublic | BindingFlags.Static);

            m_UnturnedPlayerFromPlayerMethod = m_Assembly
                .GetType("Rocket.Unturned.Player.UnturnedPlayer", false)
                .GetMethod("FromPlayer", BindingFlags.Public | BindingFlags.Static);

            Init();
        }

        public void CallRocketEventInternal(Player player, EChatMode chatMode, ref Color color, string message, ref bool cancel)
        {
            if (m_UnturnedPlayerEventsFirePlayerChattedMethod == null || m_UnturnedPlayerFromPlayerMethod == null)
            {
                return;
            }
            try
            {
                var uPlayer = m_UnturnedPlayerFromPlayerMethod.Invoke(null, new object[] { player });
                var parameters = new object[]
                {
                uPlayer,
                chatMode,
                color,
                message,
                cancel
                };

                color = ((UnityEngine.Color)m_UnturnedPlayerEventsFirePlayerChattedMethod.Invoke(null, parameters)).ToSystemColor();
                cancel = (bool)parameters[4];
            }
            catch(Exception e)
            {
                m_Logger.LogError(e, "The error occurred when calling the event UnturnedChat.OnPlayerChatted");
            }
            
        }

        private void Init()
        {
            try
            {
                var orgMethod = m_Assembly
                    .GetType("Rocket.Unturned.Chat.UnturnedChat", false)
                    .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);

                if (orgMethod == null)
                {
                    throw new Exception("Couldn't found a \"Awake\" method in UnturnedChat.");
                }

                m_Harmony.NopPatch(orgMethod);
            }
            catch (Exception e)
            {
                m_Logger.LogError(e, "Failed to patch UnturnedChat method {method}. Report about that here: https://discord.gg/6KymqGv",
                    "Awake");
            }
        }
    }
}
