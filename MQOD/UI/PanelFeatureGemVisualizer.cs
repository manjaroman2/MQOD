using System;
using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelFeatureGemVisualizer : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<float> gemRadiusColorFloatEntry;
        public readonly MelonPreferences_Entry<KeyCode?> gemRadiusVisualizerToggleKeyEntry;
        public readonly MelonPreferences_Entry<float> widthModifierEntry;

        public Action<Color> setToggleColor;

        public PanelFeatureGemVisualizer(UIBaseMQOD owner) : base(owner)
        {
            widthModifierEntry = prefManager.addSettingsEntry("gemVisualizerWidthModifierEntry", 0.5f);
            gemRadiusColorFloatEntry = prefManager.addSettingsEntry("gemRadiusColorFloat", 592.0f);
            widthModifierEntry.OnEntryValueChanged.Subscribe((_, newValue) =>
            {
                if (MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator != null)
                    MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator.updateWidth(newValue);
            });
            gemRadiusVisualizerToggleKeyEntry =
                prefManager.addHotkeyEntry("gemRadiusVisualizerToggleKeyEntry");
        }

        public override string Name => "MQOD - Gem Visualizer";
        public override int MinWidth => 600;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void LateConstructUI()
        {
            createHotkeyToggle(() => gemRadiusVisualizerToggleKeyEntry.Value,
                code => gemRadiusVisualizerToggleKeyEntry.Value = code,
                out Action<Color> setColor);
            setToggleColor = setColor;
            setColor(MQOD.Instance.GemRadiusVisualizerInst.Shown.Value ? Color.green : Color.red);
            createSlider("Width Modifier", 0.01f, 1.00f,
                f => { widthModifierEntry.Value = f; },
                () => widthModifierEntry.Value);
            createColorSlider("Color", f => { gemRadiusColorFloatEntry.Value = f; },
                () => gemRadiusColorFloatEntry.Value);
            createDropdown("Shader", MQOD.Instance.GemRadiusVisualizerInst.ShaderOptions,
                i => { MQOD.Instance.GemRadiusVisualizerInst.ShaderNumber.Value = i; },
                MQOD.Instance.GemRadiusVisualizerInst.ShaderNumber.Value);
            base.LateConstructUI();
        }
    }
}