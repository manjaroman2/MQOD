using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelMain : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<KeyCode?> toggleUIKeyEntry;

        public PanelMain(UIBaseMQOD owner) : base(owner)
        {
            toggleUIKeyEntry = prefManager.addHotkeyEntry("toggleUIKeyEntry", KeyCode.U);
        }

        public override string Name => "MQOD - Settings";
        public override int MinWidth => 500;
        public override int MinHeight => 500;
        public override Vector2 DefaultAnchorMin => new(0.00f, 1.00f);
        public override Vector2 DefaultAnchorMax => new(0.00f, 1.00f);
        public override bool CanDragAndResize => true;

        protected override void LateConstructUI()
        {
            createHotkey("UIToggle", toggleUIKeyEntry);
            createPanelSwitch("Custom Sort", MQOD.Instance.UIInst.FeatureSort);
            createPanelSwitch("Minimap", MQOD.Instance.UIInst.FeatureMinimap);

            createPanelSwitch("Gem Visualizer", MQOD.Instance.UIInst.FeatureGemVisualizer);
            createPanelSwitch("Camera Zoom", MQOD.Instance.UIInst.FeatureCamera);
            base.LateConstructUI();
        }
    }
}