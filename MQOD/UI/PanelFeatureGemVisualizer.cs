using UnityEngine;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureGemVisualizer : PanelBaseMQOD
    {
        public PanelFeatureGemVisualizer(UIBase owner) : base(owner)
        {
        }

        public override string Name => "MQOD - Gem Visualizer";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();
            createSlider("Width", 0.01f, 2.00f,
                f => { MQOD.Instance.GemRadiusVisualizerInst.gemVisualizerWidth = f; },
                () => MQOD.Instance.GemRadiusVisualizerInst.gemVisualizerWidth);
        }
    }
}