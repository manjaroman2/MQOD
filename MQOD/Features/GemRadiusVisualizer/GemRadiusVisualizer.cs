using System;
using System.Reflection;
using Death.Run.Behaviours.Entities;
using Death.Run.Behaviours.Players;
using Death.Run.Core;
using Death.Run.Core.Talents;
using Death.Run.UserInterface.CharacterStats;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class GemRadiusVisualizer : _Feature
    {
        private static readonly PropertyInfo BGC_Stats_Accessor =
            AccessTools.Property(typeof(Behaviour_GemCollector), "Stats");

        private static readonly PropertyInfo GUI_CharacterStats_Defensive_Accessor =
            AccessTools.Property(typeof(GUI_CharacterStats), "Defensive");

        private static readonly FieldInfo _isPrimaryPlayerInstance_Accessor =
            AccessTools.Field(typeof(Behaviour_Player), "_isPrimaryPlayerInstance");

        public readonly MelonPreferences_Entry<int> ShaderNumber =
            MQOD.Instance.preferencesManager.addSettingsEntry("GemRadiusVisualizerShaderNumber", 0);

        public readonly string[] ShaderOptions = { "SimpleCircleShader", "CircleShaderPixel" };

        public readonly MelonPreferences_Entry<bool> Shown =
            MQOD.Instance.preferencesManager.addSettingsEntry("GemRadiusVisualizerShown", true);

        private Behaviour_GemCollector behaviourGemCollector;
        public GemRadiusCreator GemRadiusCreator;
        public float PullArea;
        public RuntimeStats Stats;

        public void toggle()
        {
            Shown.Value = !Shown.Value;
            if (GemRadiusCreator != null) GemRadiusCreator.quad.SetActive(Shown.Value);
            MQOD.Instance.UIInst.FeatureGemVisualizer.setToggleColor(Shown.Value ? Color.green : Color.red);
        }

        public void updateScale()
        {
            GemRadiusCreator.Scale = Mathf.Max(Stats.GetAsRadius(StatId.PullArea), 0.15f) * 4.0f;
        }

        private void getBehaviourGemCollector(Behaviour_Player behaviourPlayer)
        {
            if (behaviourGemCollector != null) return; // load only if stale
            behaviourGemCollector = behaviourPlayer.GetComponent<Behaviour_GemCollector>();
            if (behaviourGemCollector == null) MelonLogger.Error("GemRadiusVisualizer: behaviourGemCollector is null!");
        }

        private static void GUI_CharacterStats__FormatPullArea__Postfix(GUI_CharacterStats __instance)
        {
            if (MQOD.Instance.GemRadiusVisualizerInst != null)
            {
                GemRadiusVisualizer gemRadiusVisualizer = MQOD.Instance.GemRadiusVisualizerInst;
                IReadOnlyRuntimeStats Defensive =
                    (IReadOnlyRuntimeStats)GUI_CharacterStats_Defensive_Accessor.GetValue(__instance, null);
                if (Defensive == null)
                {
                    MelonLogger.Error("GUI_CharacterStats__FormatPullArea__Postfix Defensive null!");
                    return;
                }

                float curValue = Defensive.Get(StatId.PullArea);
                if (!Mathf.Approximately(curValue, gemRadiusVisualizer.PullArea))
                {
                    if (gemRadiusVisualizer.Stats != null)
                    {
                        if (gemRadiusVisualizer.GemRadiusCreator == null)
                        {
                            MelonLogger.Error("EllipseCreator is null!");
                            return;
                        }

                        gemRadiusVisualizer.PullArea = curValue;
                        gemRadiusVisualizer.updateScale();
                    }
                    else
                    {
                        MelonLogger.Msg("GemRadiusVisualizerInst.Stats is null");
                    }
                }
            }
        }

        private static void Behaviour_Player__Init__Postfix(TalentLoadout talents, Action onEquipmentChangeCallback,
            Behaviour_Player __instance)
        {
            if ((bool)_isPrimaryPlayerInstance_Accessor.GetValue(__instance))
            {
                GemRadiusVisualizer gemRadiusVisualizer = MQOD.Instance.GemRadiusVisualizerInst;
                if (gemRadiusVisualizer == null)
                {
                    MelonLogger.Error("Behaviour_Player__Init__Postfix: gemRadiusVisualizer == null");
                    return;
                }

                gemRadiusVisualizer.getBehaviourGemCollector(__instance);
                GameObject playerFab = gemRadiusVisualizer.behaviourGemCollector.gameObject;
                if (gemRadiusVisualizer.GemRadiusCreator == null ||
                    playerFab.GetComponent<GemRadiusCreator>() == null)
                {
                    gemRadiusVisualizer.Stats =
                        (RuntimeStats)BGC_Stats_Accessor.GetValue(__instance, null);
                    GemRadiusCreator gemRadiusCreator = playerFab.AddComponent<GemRadiusCreator>();
                    gemRadiusCreator.setParentObject(playerFab);
                    gemRadiusVisualizer.GemRadiusCreator = gemRadiusCreator;
                    gemRadiusCreator.enabled = true;
                    gemRadiusVisualizer.updateScale();
                }
            }
        }

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(Behaviour_Player), nameof(Behaviour_Player.Init),
                new[] { typeof(TalentLoadout), typeof(Action) }, postfixClazz: typeof(GemRadiusVisualizer),
                postfixMethod: nameof(Behaviour_Player__Init__Postfix));
            HarmonyHelper.Patch(typeof(GUI_CharacterStats), "FormatPullArea",
                postfixClazz: typeof(GemRadiusVisualizer),
                postfixMethod: nameof(GUI_CharacterStats__FormatPullArea__Postfix));
        }
    }
}