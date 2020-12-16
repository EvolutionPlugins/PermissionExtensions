using HarmonyLib;
using Serilog;
using System;
using System.Linq;
using System.Reflection;

namespace PermissionExtensions
{
    public static class RocketModHandleChatPatch
    {
        private static bool IsInited;

        public static void Init(Harmony harmony)
        {
            if (IsInited)
            {
                return;
            }

            try
            {
                var orgMethod = AppDomain.CurrentDomain.GetAssemblies().First(d =>
                    d.GetName().Name.Contains("Rocket.Unturned")) // Contains because we are using HotReloading
                    .GetType("Rocket.Unturned.Chat.UnturnedChat")
                    .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);

                var ptcMethod = typeof(RocketModHandleChatPatch).GetMethod("handleChat", BindingFlags.Static | BindingFlags.NonPublic);

                var processor = harmony.CreateProcessor(orgMethod);
                processor.AddPrefix(new HarmonyMethod(ptcMethod));
                processor.Patch();

                IsInited = true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Something goes wrong when patching Rocket method \"Awake\". Report about that here: https://discord.gg/6KymqGv");
                throw;
            }
        }

        private static bool handleChat()
        {
            return false;
        }

        public static bool IsRocketInstalled()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(d => d.GetName().Name.Equals("Rocket.Unturned"));
        }
    }
}
