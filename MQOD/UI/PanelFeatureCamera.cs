using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelFeatureCamera : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<KeyCode?> cameraZoomKeyEntry;

        public PanelFeatureCamera(UIBaseMQOD owner) : base(owner)
        {
            cameraZoomKeyEntry = prefManager.addHotkeyEntry("cameraZoomKeyEntry");
        }

        public override string Name => "MQOD - Camera Settings";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void LateConstructUI()
        {
            createHotkey("Camera Zoom", () => cameraZoomKeyEntry.Value, code => cameraZoomKeyEntry.Value = code);
            base.LateConstructUI();
        }
    }
}