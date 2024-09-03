using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace MQOD
{
    public class UniverseLibHooks : Hookable
    {
        private readonly Dictionary<Type, Action<PanelBase>> ConstructorPrefixAction; 
        public UniverseLibHooks(Dictionary<Type, Action<PanelBase>> constructorPrefixAction)
        {
            ConstructorPrefixAction = constructorPrefixAction;
        }

        private static void PanelBase_Constructor_Prefix(UIBase owner, PanelBase __instance)
        {
            if (MQOD.Instance.UniverseLibHooksInst.ConstructorPrefixAction.ContainsKey(__instance.GetType()))
            {
                MelonLogger.Msg("PanelBase_Constructor_Prefix: " + __instance.GetType());
                MQOD.Instance.UniverseLibHooksInst.ConstructorPrefixAction[__instance.GetType()](__instance);
            }
        }

        public void addHarmonyHooks()
        {
            HarmonyHelper.PatchConstructor(typeof(PanelBase), new[] { typeof(UIBase) },
                prefixClazz: typeof(UniverseLibHooks), prefixMethod: nameof(PanelBase_Constructor_Prefix));
        }
    }
}