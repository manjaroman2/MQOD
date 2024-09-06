using System.Timers;
using UnityEngine;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelMain : PanelBaseMQOD
    {
        public readonly Timer toggleUITimer = new(1000);

        public PanelMain(UIBase owner) : base(owner)
        {
        }

        public override string Name => "MQOD - Settings";
        public override int MinWidth => 500;
        public override int MinHeight => 500;
        public override Vector2 DefaultAnchorMin => new(0.00f, 1.00f);
        public override Vector2 DefaultAnchorMax => new(0.00f, 1.00f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();

            createHotkey("UIToggle", () => MQOD.Instance.UI.toggleUIKey,
                code => MQOD.Instance.UI.toggleUIKey = code,
                toggleUITimer);
            createPanelSwitch("Custom Sort", MQOD.Instance.UI.FeatureSort);
            createPanelSwitch("Minimap", MQOD.Instance.UI.FeatureMinimap);

            createPanelSwitch("Gem Visualizer", MQOD.Instance.UI.FeatureGemVisualizer);
            createPanelSwitch("Camera Zoom", MQOD.Instance.UI.FeatureCamera);
        }
    }
}