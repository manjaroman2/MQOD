using System;
using System.Collections.Generic;
using MelonLoader;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace MQOD
{
    public class UniverseLibHooks : _Hookable
    {
        private readonly Dictionary<Type, Action<PanelBase>> ConstructorPrefixAction;

        public UniverseLibHooks()
        {
            ConstructorPrefixAction = new Dictionary<Type, Action<PanelBase>>
            {
                {
                    typeof(PanelSort), pBase =>
                    {
                        PanelSort panelSort = (PanelSort)pBase;
                        panelSort.loadVariables(MQOD.Instance.preferencesManager.customSortOrderingEntry.Value);
                    }
                }
            };
        }

        public void addHarmonyHooks()
        {
            HarmonyHelper.PatchConstructor(typeof(PanelBase), new[] { typeof(UIBase) },
                typeof(UniverseLibHooks), nameof(PanelBase_Constructor_Prefix));
        }

        private static void PanelBase_Constructor_Prefix(UIBase owner, PanelBase __instance)
        {
            if (MQOD.Instance.UniverseLibHooksInst.ConstructorPrefixAction.ContainsKey(__instance.GetType()))
            {
                MelonLogger.Msg("PanelBase_Constructor_Prefix: " + __instance.GetType());
                MQOD.Instance.UniverseLibHooksInst.ConstructorPrefixAction[__instance.GetType()](__instance);
            }
        }
    }
}