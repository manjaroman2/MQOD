using System.Reflection;
using Death.App.MainMenu;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public class AutoStart : _Feature
    {
        private static readonly MethodInfo OnStartClick_Accessor =
            AccessTools.Method(typeof(Facade_MainMenu), "OnStartClick");

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(Facade_MainMenu), "Start", postfixClazz: typeof(AutoStart),
                postfixMethod: nameof(Facade_MainMenu__Start__Postfix));
        }

        private static void Facade_MainMenu__Start__Postfix(Facade_MainMenu __instance)
        {
            MelonLogger.Warning("Autostart!");
            OnStartClick_Accessor.Invoke(__instance, null);
        }
    }
}