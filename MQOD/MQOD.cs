using System;
using System.Collections.Generic;
using Claw.UserInterface.Screens;
using Death.TimesRealm;
using Death.TimesRealm.UserInterface;
using Death.UserInterface;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace MQOD
{
    public class MQOD : MelonMod
    {
        public static MQOD Instance;
        public StashSort StashSortInst;
        public ShopSort ShopSortInst;
        public ArmorySort ArmorySortInst;
        public SortedItemGrid SortedItemGridInst;
        public BetterMinimap BetterMinimapInst;
        public UniverseLibHooks UniverseLibHooksInst;
        public bool IsRun;
        public ScreenManager ScreenManager;


        private readonly FeatureManager featureManager = new();
        public readonly PreferencesManager preferencesManager = new();
        public MQOD_UI mqodUI;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Hello from MoreQOD!");
            Instance = this;

            preferencesManager.init();

            UniverseLibHooksInst = new UniverseLibHooks(new Dictionary<Type, Action<PanelBase>>()
            {
                {
                    typeof(MQOD_UI.CustomSortPanel), (pBase =>
                    {
                        MQOD_UI.CustomSortPanel customSortPanel = (MQOD_UI.CustomSortPanel)pBase;
                        customSortPanel.loadVariables(Instance.preferencesManager.customSortOrderingEntry.Value);
                    })
                }
            });
            UniverseLibHooksInst.addHarmonyHooks();

            mqodUI = new MQOD_UI();
            mqodUI.init();


            SortedItemGridInst = featureManager.addFeature<SortedItemGrid>();
            StashSortInst = featureManager.addFeature<StashSort>();
            ShopSortInst = featureManager.addFeature<ShopSort>();
            ArmorySortInst = featureManager.addFeature<ArmorySort>();
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

            if (mqodUI.minimapFullscreenKey != null)
            {
                if (!mqodUI.MinimapZoomFunction)
                {
                    if (Input.GetKeyDown((KeyCode)mqodUI.minimapFullscreenKey)) BetterMinimapInst.fullscreenMinimap();
                    else if (Input.GetKeyUp((KeyCode)mqodUI.minimapFullscreenKey)) BetterMinimapInst.resetFullscreen();
                }
                else
                {
                    if (Input.GetKeyDown((KeyCode)mqodUI.minimapFullscreenKey))
                    {
                        if (!BetterMinimapInst.zoomedIn) BetterMinimapInst.fullscreenMinimap();
                        else BetterMinimapInst.resetFullscreen();
                    }
                }
            }

            if (mqodUI.minimapZoomOutKey != null && Input.GetKeyDown((KeyCode)mqodUI.minimapZoomOutKey))
                BetterMinimapInst.zoomOut();
            if (mqodUI.minimapZoomInKey != null && Input.GetKeyDown((KeyCode)mqodUI.minimapZoomInKey))
                BetterMinimapInst.zoomIn();

            if (mqodUI.toggleAutoSortingKey != null &&
                Input.GetKeyDown((KeyCode)mqodUI.toggleAutoSortingKey)) // middle click or S 
            {
                SortedItemGridInst.toggleSorting();
                Color color;
                string text;
                if (SortedItemGridInst.isEnabled())
                {
                    color = Color.green;
                    text = "enabled";
                }
                else
                {
                    color = Color.red;
                    text = "disabled";
                }

                if (mqodUI.HotkeyPanel.toggleAutoSortingLabel != null)
                {
                    mqodUI.HotkeyPanel.toggleAutoSortingLabel.color = color;
                    mqodUI.HotkeyPanel.toggleAutoSortingLabel.text = $"toggleAutoSorting [{text}]";
                }

                MelonLogger.Msg($"Item grid sorting: {text}");
            }

            if (mqodUI.sortingKey != null && Input.GetKeyDown((KeyCode)mqodUI.sortingKey) && ScreenManager != null)
            {
                switch (ScreenManager.CurrentScreen)
                {
                    case Screen_Stash:
                        StashSortInst.sortSelectedPage();
                        break;
                    case Screen_Shop:
                        ShopSort.sortShop();
                        break;
                    case Screen_Armory:
                        ArmorySortInst.sort();
                        break;
                }
            }


            if (mqodUI.toggleUIKey != null && mqodUI.initialized && !mqodUI.HotkeyPanel.toggleUITimer.Enabled &&
                Input.GetKeyDown((KeyCode)mqodUI.toggleUIKey))
            {
                MelonLogger.Msg("Toggle UI");
                mqodUI.UIBase.Enabled = !mqodUI.UIBase.Enabled;
            }
        }

        private static void Facade_Lobby__Init__Postfix(ILobbyGameState state, Facade_Lobby __instance)
        {
            GUIManager guiManager = GameObject.FindWithTag("RunGUI").GetComponent<GUIManager>();
            Instance.ScreenManager = guiManager.GetComponent<ScreenManager>();
            if (!Instance.ScreenManager)
            {
                MelonLogger.Error("ScreenManager is null, something went terribly wrong!");
            }
        }
    }
}