using HarmonyLib;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Patching;
using System;
using System.Reflection;

namespace PermissionExtensions
{
    public class RocketModHandleChatPatch
    {
        private readonly ILogger<PermissionExtensions> m_Logger;
        private readonly Assembly m_Assembly;
        private readonly Harmony m_Harmony;

        public RocketModHandleChatPatch(ILogger<PermissionExtensions> logger, Harmony harmony, Assembly assembly)
        {
            m_Logger = logger;
            m_Harmony = harmony;
            m_Assembly = assembly;

            Init();
        }

        private void Init()
        {
            try
            {
                var orgMethod = m_Assembly
                    .GetType("Rocket.Unturned.Chat.UnturnedChat")
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
