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
        private const float startupDelay = 0f;
        public PanelFeatureCamera FeatureCamera;
        public PanelFeatureGemVisualizer FeatureGemVisualizer;
        public PanelFeatureMinimap FeatureMinimap;
        public PanelFeatureSort FeatureSort;
        public bool initialized;
        public PanelMain Main;
        public UIBase UIBase;
        public int fontSize { get; set; }


        public KeyCode? toggleUIKey
        {
            get => MQOD.Instance.preferencesManager.toggleUIKeyEntry.Value;
            set => MQOD.Instance.preferencesManager.toggleUIKeyEntry.Value = value;
        }


        public void init()
        {
            MelonLogger.Msg("UIMQOD init");
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
            MelonLogger.Msg("UIMQOD OnInitialized");
            UIBase = UniversalUI.RegisterUI("mj.MQOD", UiUpdate);
            FeatureSort = new PanelFeatureSort(UIBase) { Enabled = false };
            FeatureGemVisualizer = new PanelFeatureGemVisualizer(UIBase) { Enabled = false };
            FeatureMinimap = new PanelFeatureMinimap(UIBase) { Enabled = false };
            FeatureCamera = new PanelFeatureCamera(UIBase) { Enabled = false };
            Main = new PanelMain(UIBase);

            CanvasScaler canvasScaler = UIBase.Canvas.gameObject.GetComponent<CanvasScaler>();
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