using UnityEngine;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureGemVisualizer : PanelBaseMQOD
    {
        public PanelFeatureGemVisualizer(UIBase owner) : base(owner)
        {
        }


        public static float widthModifier
        {
            get => MQOD.Instance.preferencesManager.widthModifier.Value;
            set
            {
                MQOD.Instance.preferencesManager.widthModifier.Value = value;
                if (MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator != null)
                    MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator.updateWidth();
            }
        }

        public static float colorFloat
        {
            get => MQOD.Instance.preferencesManager.gemRadiusColorFloat.Value;
            set => MQOD.Instance.preferencesManager.gemRadiusColorFloat.Value = value;
        }

        public override string Name => "MQOD - Gem Visualizer";
        public override int MinWidth => 600;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();
            createHotkey("Toggle", () => MQOD.Instance.preferencesManager.gemRadiusVisualizerToggleKeyEntry.Value,
                code => MQOD.Instance.preferencesManager.gemRadiusVisualizerToggleKeyEntry.Value = code);
            createSlider("Width Modifier", 0.01f, 1.00f,
                f => { widthModifier = f; },
                () => widthModifier);
            createColorSlider("Color", f => { colorFloat = f; }, () => colorFloat);
        }
    }
}