using System;
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
        public StashSort StashSortInst;
        public ShopSort ShopSortInst;
        public ArmorySort ArmorySortInst;
        public SortedItemGrid SortedItemGridInst;
        public bool IsRun;
        public ScreenManager ScreenManager;
        public KeyCode? sortingKey = KeyCode.S;
        public KeyCode? toggleAutoSortingKey = KeyCode.P;
        public KeyCode? toggleUIKey = KeyCode.U;

        private readonly FeatureManager featureManager = new();
        private MQOD_UI mqodUI;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Hello from MoreQOD!");
            Instance = this;
            mqodUI = new MQOD_UI();
            mqodUI.init();

            SortedItemGridInst = featureManager.addFeature<SortedItemGrid>();
            StashSortInst = featureManager.addFeature<StashSort>();
            ShopSortInst = featureManager.addFeature<ShopSort>();
            ArmorySortInst = featureManager.addFeature<ArmorySort>();
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
                    break;
                case "Scene_TimesRealm":
                    IsRun = false;
                    break;
                case "Scene_LobbyGUI":
                    break;
                case "Scene_Bootstrap":
                    // dGameManager = SceneManager.GetActiveScene().GetRootGameObjects()[0].GetComponent<GameManager>();
                    break;
                default:
                    break;
            }

            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
        }

        public override void OnLateUpdate()
        {
            if (toggleAutoSortingKey != null && Input.GetKeyDown((KeyCode) toggleAutoSortingKey)) // middle click or S 
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

            if (sortingKey != null && Input.GetKeyDown((KeyCode)sortingKey) && ScreenManager != null)
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
                    default:
                        // MelonLogger.Msg(ScreenManager.CurrentScreen);
                        break;
                }
            }
            
            
            if (toggleUIKey != null && mqodUI.initialized && !mqodUI.HotkeyPanel.toggleUITimer.Enabled &&
                Input.GetKeyDown((KeyCode) toggleUIKey))
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