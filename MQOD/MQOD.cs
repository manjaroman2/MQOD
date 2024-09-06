using System;
using Claw.UserInterface.Screens;
using Death.Run.Behaviours;
using Death.TimesRealm;
using Death.TimesRealm.UserInterface;
using Death.UserInterface;
using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class MQOD : MelonMod
    {
        private static MQOD _Instance; 
        public static MQOD Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                MelonLogger.Error("MQOD _Instance is null!");
                throw new NullReferenceException("MQOD _Instance is null!");
            }
        }


        public readonly AssetManager assetManager = new();
        private readonly FeatureManager featureManager = new();
        public readonly PreferencesManager preferencesManager = new();
        public BetterMinimap BetterMinimapInst;
        public CameraZoom CameraZoomInst;
        public GemRadiusVisualizer GemRadiusVisualizerInst;
        public bool IsRun;
        public ScreenManager ScreenManager;

        public SortArmory SortArmoryInst;
        public SortItemGrid SortItemGridInst;
        public SortShop SortShopInst;
        public SortStash SortStashInst;
        public UIMQOD UI;
        public UniverseLibHooks UniverseLibHooksInst;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Hello from MoreQOD.");
            _Instance = this;

            assetManager.init();
            preferencesManager.init();
            UniverseLibHooksInst = new UniverseLibHooks();
            UniverseLibHooksInst.applyHarmonyHooks();
            UI = new UIMQOD();
            UI.init();

            SortItemGridInst = featureManager.addFeature<SortItemGrid>();
            SortStashInst = featureManager.addFeature<SortStash>();
            SortShopInst = featureManager.addFeature<SortShop>();
            SortArmoryInst = featureManager.addFeature<SortArmory>();
            BetterMinimapInst = featureManager.addFeature<BetterMinimap>();
            GemRadiusVisualizerInst = featureManager.addFeature<GemRadiusVisualizer>();
            CameraZoomInst = featureManager.addFeature<CameraZoom>();
            featureManager.addHarmonyHooks();


            HarmonyHelper.Patch(typeof(Facade_Lobby), nameof(Facade_Lobby.Init), new[] { typeof(ILobbyGameState) },
                postfixClazz: typeof(MQOD), postfixMethod: nameof(Facade_Lobby__Init__Postfix));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "Scene_Run":
                    IsRun = true;
                    break;
                case "Scene_RunGUI":
                    BetterMinimapInst.init();
                    break;
                case "Scene_TimesRealm":
                    IsRun = false;
                    break;
                case "Scene_LobbyGUI":
                    break;
                case "Scene_Bootstrap":
                    // dGameManager = SceneManager.GetActiveScene().GetRootGameObjects()[0].GetComponent<GameManager>();
                    break;
            }

            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
        }

        public override void OnLateUpdate()
        {
            if (!UI.initialized) return;
            if (Input.GetKeyDown(KeyCode.L)) Player.Instance.Entity.Invulnerable.AddStack();
            // if (Input.GetKeyDown(KeyCode.K))
            //     MelonLogger.Msg("Width: " + Instance.GemRadiusVisualizerInst.GemRadiusCreator.quad
            //         .GetComponent<Renderer>().material.GetFloat(GemRadiusCreator.__Width));

            if (UI.FeatureMinimap.minimapFullscreenKey != null)
            {
                if (!UI.FeatureMinimap.MinimapZoomFunction)
                {
                    if (Input.GetKeyDown((KeyCode)UI.FeatureMinimap.minimapFullscreenKey))
                        BetterMinimapInst.fullscreenMinimap();
                    else if (Input.GetKeyUp((KeyCode)UI.FeatureMinimap.minimapFullscreenKey))
                        BetterMinimapInst.resetFullscreen();
                }
                else
                {
                    if (Input.GetKeyDown((KeyCode)UI.FeatureMinimap.minimapFullscreenKey))
                    {
                        if (!BetterMinimapInst.zoomedIn) BetterMinimapInst.fullscreenMinimap();
                        else BetterMinimapInst.resetFullscreen();
                    }
                }
            }

            if (UI.FeatureMinimap.minimapZoomOutKey != null &&
                Input.GetKeyDown((KeyCode)UI.FeatureMinimap.minimapZoomOutKey))
                BetterMinimapInst.zoomOut();
            if (UI.FeatureMinimap.minimapZoomInKey != null &&
                Input.GetKeyDown((KeyCode)UI.FeatureMinimap.minimapZoomInKey))
                BetterMinimapInst.zoomIn();

            if (UI.FeatureSort.toggleAutoSortingKey != null &&
                Input.GetKeyDown((KeyCode)UI.FeatureSort.toggleAutoSortingKey)) // middle click or S 
            {
                SortItemGridInst.toggleSorting();
                Color color;
                string text;
                if (SortItemGridInst.isEnabled())
                {
                    color = Color.green;
                    text = "enabled";
                }
                else
                {
                    color = Color.red;
                    text = "disabled";
                }

                if (UI.FeatureSort.toggleAutoSortingLabel != null)
                {
                    UI.FeatureSort.toggleAutoSortingLabel.color = color;
                    UI.FeatureSort.toggleAutoSortingLabel.text = $"toggleAutoSorting [{text}]";
                }

                MelonLogger.Msg($"Item grid sorting: {text}");
            }

            if (UI.FeatureSort.sortingKey != null && Input.GetKeyDown((KeyCode)UI.FeatureSort.sortingKey) && ScreenManager != null)
                switch (ScreenManager.CurrentScreen)
                {
                    case Screen_Stash:
                        SortStashInst.sortSelectedPage();
                        break;
                    case Screen_Shop:
                        SortShop.sortShop();
                        break;
                    case Screen_Armory:
                        SortArmoryInst.sort();
                        break;
                }

            if (UI.FeatureCamera.cameraZoomKey != null && Input.GetKeyDown((KeyCode)UI.FeatureCamera.cameraZoomKey))
            {
                CameraZoomInst.zoomOut();
            }

            if (preferencesManager.gemRadiusVisualizerToggleKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)preferencesManager.gemRadiusVisualizerToggleKeyEntry.Value))
            {
                GemRadiusVisualizerInst.toggle(); 
            }

            if (UI.toggleUIKey != null && UI.initialized && !UI.Main.toggleUITimer.Enabled &&
                Input.GetKeyDown((KeyCode)UI.toggleUIKey))
            {
                // MelonLogger.Msg("Toggle UI");
                UI.UIBase.Enabled = !UI.UIBase.Enabled;
            }
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("Quitting!");
            MelonPreferences.Save();
            base.OnApplicationQuit();
        }

        private static void Facade_Lobby__Init__Postfix(ILobbyGameState state, Facade_Lobby __instance)
        {
            GUIManager guiManager = GameObject.FindWithTag("RunGUI").GetComponent<GUIManager>();
            Instance.ScreenManager = guiManager.GetComponent<ScreenManager>();
            if (!Instance.ScreenManager) MelonLogger.Error("ScreenManager is null, something went terribly wrong!");
        }
    }
}