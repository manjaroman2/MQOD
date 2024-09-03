using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelHotkey : PanelBaseMQOD
    {
        public readonly Timer toggleUITimer = new(1000);
        public Text toggleAutoSortingLabel;

        public PanelHotkey(UIBase owner) : base(owner)
        {
        }

        public override string Name => "MQOD - Hotkeys";
        public override int MinWidth => 300;
        public override int MinHeight => 500;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, childControlWidth: true,
                childControlHeight: true, forceHeight: false, forceWidth: false, padBottom: 20, padLeft: 20,
                padRight: 20, spacing: 5);
            createHotkey(0, "UIToggle", () => MQOD.Instance.UI.toggleUIKey,
                code => MQOD.Instance.UI.toggleUIKey = code,
                toggleUITimer);
            createHotkey(1, "Sorting", () => MQOD.Instance.UI.sortingKey,
                code => MQOD.Instance.UI.sortingKey = code);
            toggleAutoSortingLabel = createHotkey(2, "toggleAutoSorting [enabled]",
                () => MQOD.Instance.UI.toggleAutoSortingKey,
                code => MQOD.Instance.UI.toggleAutoSortingKey = code);
            createSwitch(3, "Custom Sort Settings", Color.yellow, Color.gray,
                () => MQOD.Instance.UI.customSortSettingsExpanded,
                () =>
                {
                    MQOD.Instance.UI.customSortSettingsExpanded =
                        !MQOD.Instance.UI.customSortSettingsExpanded;
                    MQOD.Instance.UI.Sort.Enabled = MQOD.Instance.UI.customSortSettingsExpanded;
                },
                () => MQOD.Instance.UI.customSortSettingsExpanded ? "Expanded" : "Hidden");
            createHotkey(4, "Fullscreen Minimap", () => MQOD.Instance.UI.minimapFullscreenKey,
                code => MQOD.Instance.UI.minimapFullscreenKey = code);
            createHotkey(5, "minimapZoomOut", () => MQOD.Instance.UI.minimapZoomOutKey,
                code => MQOD.Instance.UI.minimapZoomOutKey = code);
            createHotkey(6, "minimapZoomIn", () => MQOD.Instance.UI.minimapZoomInKey,
                code => MQOD.Instance.UI.minimapZoomInKey = code);

            createSwitch(7, "Minimap Function", Color.gray, Color.gray,
                () => MQOD.Instance.UI.MinimapZoomFunction,
                () => MQOD.Instance.UI.MinimapZoomFunction = !MQOD.Instance.UI.MinimapZoomFunction,
                () => MQOD.Instance.UI.MinimapZoomFunction ? "Toggle" : "Hold");

            createSlider(8, "Minimap opacity", 0.01f, 1.00f, f =>
            {
                MQOD.Instance.UI.minimapTransparency = f;
                if (MQOD.Instance.BetterMinimapInst.initialized && MQOD.Instance.BetterMinimapInst.zoomedIn)
                {
                    Color boundsImage_color = MQOD.Instance.BetterMinimapInst.boundsImage_color;
                    MQOD.Instance.BetterMinimapInst.boundsImage.color = new Color(boundsImage_color.r,
                        boundsImage_color.g, boundsImage_color.b, MQOD.Instance.UI.minimapTransparency);
                }
            }, () => MQOD.Instance.UI.minimapTransparency);
        }
    }
}