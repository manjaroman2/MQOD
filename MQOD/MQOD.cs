using Claw.UserInterface.Screens;
using Death.TimesRealm;
using Death.TimesRealm.UserInterface;
using Death.UserInterface;
using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class MQOD : MelonMod
    {
        public static MQOD Instance;


        private readonly FeatureManager featureManager = new();
        public readonly PreferencesManager preferencesManager = new();
        public BetterMinimap BetterMinimapInst;
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
            MelonLogger.Msg("Hello from MoreQOD!");
            Instance = this;

            preferencesManager.init();

            UniverseLibHooksInst = new UniverseLibHooks();
            UniverseLibHooksInst.addHarmonyHooks();

            UI = new UIMQOD();
            UI.init();

            SortItemGridInst = featureManager.addFeature<SortItemGrid>();
            SortStashInst = featureManager.addFeature<SortStash>();
            SortShopInst = featureManager.addFeature<SortShop>();
            SortArmoryInst = featureManager.addFeature<SortArmory>();
            BetterMinimapInst = featureManager.addFeature<BetterMinimap>();
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
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     if (IsRun) Player.Instance.Entity.Invulnerable.AddStack();
            // }

            if (UI.minimapFullscreenKey != null)
            {
                if (!UI.MinimapZoomFunction)
                {
                    if (Input.GetKeyDown((KeyCode)UI.minimapFullscreenKey)) BetterMinimapInst.fullscreenMinimap();
                    else if (Input.GetKeyUp((KeyCode)UI.minimapFullscreenKey)) BetterMinimapInst.resetFullscreen();
                }
                else
                {
                    if (Input.GetKeyDown((KeyCode)UI.minimapFullscreenKey))
                    {
                        if (!BetterMinimapInst.zoomedIn) BetterMinimapInst.fullscreenMinimap();
                        else BetterMinimapInst.resetFullscreen();
                    }
                }
            }

            if (UI.minimapZoomOutKey != null && Input.GetKeyDown((KeyCode)UI.minimapZoomOutKey))
                BetterMinimapInst.zoomOut();
            if (UI.minimapZoomInKey != null && Input.GetKeyDown((KeyCode)UI.minimapZoomInKey))
                BetterMinimapInst.zoomIn();

            if (UI.toggleAutoSortingKey != null &&
                Input.GetKeyDown((KeyCode)UI.toggleAutoSortingKey)) // middle click or S 
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

                if (UI.Hotkey.toggleAutoSortingLabel != null)
                {
                    UI.Hotkey.toggleAutoSortingLabel.color = color;
                    UI.Hotkey.toggleAutoSortingLabel.text = $"toggleAutoSorting [{text}]";
                }

                MelonLogger.Msg($"Item grid sorting: {text}");
            }

            if (UI.sortingKey != null && Input.GetKeyDown((KeyCode)UI.sortingKey) && ScreenManager != null)
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


            if (UI.toggleUIKey != null && UI.initialized && !UI.Hotkey.toggleUITimer.Enabled &&
                Input.GetKeyDown((KeyCode)UI.toggleUIKey))
            {
                MelonLogger.Msg("Toggle UI");
                UI.UIBase.Enabled = !UI.UIBase.Enabled;
            }
        }

        private static void Facade_Lobby__Init__Postfix(ILobbyGameState state, Facade_Lobby __instance)
        {
            GUIManager guiManager = GameObject.FindWithTag("RunGUI").GetComponent<GUIManager>();
            Instance.ScreenManager = guiManager.GetComponent<ScreenManager>();
            if (!Instance.ScreenManager) MelonLogger.Error("ScreenManager is null, something went terribly wrong!");
        }
    }
}