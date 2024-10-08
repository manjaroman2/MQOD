using System.IO;
using System.Timers;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;

namespace MQOD
{
    public class UI
    {
        private const float startupDelay = 0f;
        public readonly Timer keyReassignTimer = new(1000) { AutoReset = false };
        public PanelFeatureCamera FeatureCamera;
        public PanelFeatureGemVisualizer FeatureGemVisualizer;
        public PanelFeatureMinimap FeatureMinimap;
        public PanelMouseEffects FeatureMouseEffects;
        public PanelFeatureSort FeatureSort;
        public bool initialized;
        public bool isAssigning = false;
        public PanelMain Main;
        public UIBaseMQOD UIBase;
        public int fontSize { get; set; }

        public void init()
        {
            MelonLogger.Msg("UI init");
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
            MelonLogger.Msg("UniverseLib initialized");
            UIBase = new UIBaseMQOD("mj.MQOD", UiUpdate, MQOD.Instance.preferencesManager);
            FeatureSort = new PanelFeatureSort(UIBase) { Enabled = false };
            FeatureGemVisualizer = new PanelFeatureGemVisualizer(UIBase) { Enabled = false };
            FeatureMinimap = new PanelFeatureMinimap(UIBase) { Enabled = false };
            FeatureCamera = new PanelFeatureCamera(UIBase) { Enabled = false };
            FeatureMouseEffects = new PanelMouseEffects(UIBase) { Enabled = false };
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