using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureGemVisualizer : PanelBaseMQOD
    {
        public PanelFeatureGemVisualizer(UIBase owner) : base(owner)
        {
            
        }


        public float widthModifier
        {
            get => MQOD.Instance.preferencesManager.widthModifier.Value;
            set
            {
                MQOD.Instance.preferencesManager.widthModifier.Value = value;
                if (MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator != null) MQOD.Instance.GemRadiusVisualizerInst.GemRadiusCreator.updateWidth();
            }
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
            createSlider("Width Modifier", 0.01f, 1.00f,
                f => { widthModifier = f; },
                () => widthModifier);
            createColorSlider("Color", f => {}, () => 0.0f);
        }
    }
}