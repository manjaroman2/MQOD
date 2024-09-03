using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.UI;

namespace MQOD
{
    public class UIMQOD
    {
        private const float startupDelay = 1f;
        public PanelHotkey Hotkey;
        public bool initialized;
        public PanelSort Sort;
        public UIBase UIBase;
        public int fontSize { get; set; }

        public KeyCode? sortingKey
        {
            get => MQOD.Instance.preferencesManager.sortingKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.sortingKeyEntry.Value = value;
        }

        public KeyCode? toggleAutoSortingKey
        {
            get => MQOD.Instance.preferencesManager.toggleAutoSortingKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.toggleAutoSortingKeyEntry.Value = value;
        }

        public KeyCode? toggleUIKey
        {
            get => MQOD.Instance.preferencesManager.toggleUIKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.toggleUIKeyEntry.Value = value;
        }

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

        public bool customSortSettingsExpanded
        {
            get => MQOD.Instance.preferencesManager.customSortSettingsExpandedEntry.Value;
            set => MQOD.Instance.preferencesManager.customSortSettingsExpandedEntry.Value = value;
        }

        public void init()
        {
            UniverseLibConfig config = new()
            {
                Disable_EventSystem_Override = false,
                Force_Unlock_Mouse = true,
                Unhollowed_Modules_Folder = Path.Combine(Path.GetDirectoryName(MelonHandler.ModsDirectory)!,
                    Path.Combine("MelonLoader", "Managed"))
            };

            Universe.Init(startupDelay, OnInitialized, LogHandler, config);
        }

        private void UiUpdate()
        {
        }

        private void OnInitialized()
        {
            UIBase = UniversalUI.RegisterUI("mj.MQOD", UiUpdate);
            Hotkey = new PanelHotkey(UIBase);
            Sort = new PanelSort(UIBase)
            {
                Enabled = MQOD.Instance.UI.customSortSettingsExpanded
            };

            CanvasScaler canvasScaler = UIBase.Canvas.gameObject.AddComponent<CanvasScaler>();
            if (canvasScaler != null) canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            UIBase.Enabled = true;
            initialized = true;
        }

        private static void LogHandler(string message, LogType type)
        {
            MelonLogger.Msg(message);
        }
    }
}