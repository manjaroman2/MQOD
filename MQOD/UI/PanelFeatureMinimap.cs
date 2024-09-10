using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelFeatureMinimap : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<KeyCode?> minimapFullscreenKeyEntry;
        public readonly MelonPreferences_Entry<float> minimapTransparencyEntry;
        public readonly MelonPreferences_Entry<bool> minimapZoomFunctionEntry;
        public readonly MelonPreferences_Entry<KeyCode?> minimapZoomInKeyEntry;
        public readonly MelonPreferences_Entry<KeyCode?> minimapZoomOutKeyEntry;

        public PanelFeatureMinimap(UIBaseMQOD owner) : base(owner)
        {
            minimapFullscreenKeyEntry = prefManager.addHotkeyEntry("minimapFullscreenEntry");
            minimapZoomOutKeyEntry = prefManager.addHotkeyEntry("minimapZoomOutEntry");
            minimapZoomInKeyEntry = prefManager.addHotkeyEntry("minimapZoomInEntry");
            minimapZoomFunctionEntry = prefManager.addSettingsEntry("minimapZoomFunctionEntry", false);
            minimapTransparencyEntry = prefManager.addSettingsEntry("minimapTransparencyEntry", 0.3f);
        }

        public char this[int index] => 'c';

        public override string Name => "MQOD - Minimap";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void LateConstructUI()
        {
            createHotkey("Fullscreen", minimapFullscreenKeyEntry);
            createHotkey("Zoom Out", minimapZoomOutKeyEntry);
            createHotkey("Zoom In", minimapZoomInKeyEntry);
            createSwitch("Fullscreen Mode", Color.gray, Color.gray,
                () => minimapZoomFunctionEntry.Value,
                () => minimapZoomFunctionEntry.Value = !minimapZoomFunctionEntry.Value,
                () => minimapZoomFunctionEntry.Value ? "Toggle" : "Hold");
            createSlider("Opacity", 0.01f, 1.00f, f =>
            {
                minimapTransparencyEntry.Value = f;
                if (MQOD.Instance.BetterMinimapInst.initialized && MQOD.Instance.BetterMinimapInst.IsFullscreen)
                {
                    Color boundsImage_color = MQOD.Instance.BetterMinimapInst.boundsImage_color;
                    MQOD.Instance.BetterMinimapInst.boundsImage.color = new Color(boundsImage_color.r,
                        boundsImage_color.g, boundsImage_color.b, minimapTransparencyEntry.Value);
                }
            }, () => minimapTransparencyEntry.Value);
            base.LateConstructUI();
        }
    }
}