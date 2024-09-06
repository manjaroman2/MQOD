using UnityEngine;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureMinimap : PanelBaseMQOD
    {
        public PanelFeatureMinimap(UIBase owner) : base(owner)
        {
        }

        public override string Name => "MQOD - Minimap";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        public KeyCode? minimapFullscreenKey
        {
            get => MQOD.Instance.preferencesManager.minimapFullscreenKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.minimapFullscreenKeyEntry.Value = value;
        }

        public KeyCode? minimapZoomOutKey
        {
            get => MQOD.Instance.preferencesManager.minimapZoomOutKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.minimapZoomOutKeyEntry.Value = value;
        }

        public KeyCode? minimapZoomInKey
        {
            get => MQOD.Instance.preferencesManager.minimapZoomInKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.minimapZoomInKeyEntry.Value = value;
        }

        public bool MinimapZoomFunction
        {
            get => MQOD.Instance.preferencesManager.minimapZoomFunctionEntry.Value;
            set => MQOD.Instance.preferencesManager.minimapZoomFunctionEntry.Value = value;
        }

        public float minimapTransparency
        {
            get => MQOD.Instance.preferencesManager.minimapTransparencyEntry.Value;
            set => MQOD.Instance.preferencesManager.minimapTransparencyEntry.Value = value;
        }

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();

            createHotkey("Fullscreen", () => minimapFullscreenKey,
                code => minimapFullscreenKey = code);
            createHotkey("Zoom Out", () => minimapZoomOutKey,
                code => minimapZoomOutKey = code);
            createHotkey("Zoom In", () => minimapZoomInKey,
                code => minimapZoomInKey = code);
            createSwitch("Fullscreen Mode", Color.gray, Color.gray,
                () => MinimapZoomFunction,
                () => MinimapZoomFunction = !MinimapZoomFunction,
                () => MinimapZoomFunction ? "Toggle" : "Hold");
            createSlider("Opacity", 0.01f, 1.00f, f =>
            {
                minimapTransparency = f;
                if (MQOD.Instance.BetterMinimapInst.initialized && MQOD.Instance.BetterMinimapInst.zoomedIn)
                {
                    Color boundsImage_color = MQOD.Instance.BetterMinimapInst.boundsImage_color;
                    MQOD.Instance.BetterMinimapInst.boundsImage.color = new Color(boundsImage_color.r,
                        boundsImage_color.g, boundsImage_color.b, minimapTransparency);
                }
            }, () => minimapTransparency);
        }
    }
}