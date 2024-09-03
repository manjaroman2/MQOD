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
        public BetterMinimap BetterMinimapInst;
        public bool IsRun;
        public ScreenManager ScreenManager;

        public KeyCode? sortingKey
        {
            get => sortingKeyEntry.Value;
            set => sortingKeyEntry.Value = value;
        }

        public KeyCode? toggleAutoSortingKey
        {
            get => toggleAutoSortingKeyEntry.Value;
            set => toggleAutoSortingKeyEntry.Value = value;
        }

        public KeyCode? toggleUIKey
        {
            get => toggleUIKeyEntry.Value;
            set => toggleUIKeyEntry.Value = value;
        }

        public KeyCode? minimapFullscreen
        {
            get => minimapFullscreenEntry.Value;
            set => minimapFullscreenEntry.Value = value;
        }

        public KeyCode? minimapZoomOut
        {
            get => minimapZoomOutEntry.Value;
            set => minimapZoomOutEntry.Value = value;
        }

        public KeyCode? minimapZoomIn
        {
            get => minimapZoomInEntry.Value;
            set => minimapZoomInEntry.Value = value;
        }

        public MQOD_UI.Minimap_ZoomFunction MinimapZoomFunction
        {
            get => minimapZoomFunctionEntry.Value;
            set => minimapZoomFunctionEntry.Value = value;
        }

        public float minimapTransparency
        {
            get => minimapTransparencyEntry.Value;
            set => minimapTransparencyEntry.Value = value;
        }

        private MelonPreferences_Category Hotkeys;
        private MelonPreferences_Entry<KeyCode?> sortingKeyEntry;
        private MelonPreferences_Entry<KeyCode?> toggleAutoSortingKeyEntry;
        private MelonPreferences_Entry<KeyCode?> toggleUIKeyEntry;
        private MelonPreferences_Entry<KeyCode?> minimapFullscreenEntry;
        private MelonPreferences_Entry<KeyCode?> minimapZoomOutEntry;
        private MelonPreferences_Entry<KeyCode?> minimapZoomInEntry;
        private MelonPreferences_Category Settings;
        private MelonPreferences_Entry<MQOD_UI.Minimap_ZoomFunction> minimapZoomFunctionEntry;
        private MelonPreferences_Entry<float> minimapTransparencyEntry;

        private readonly FeatureManager featureManager = new();
        private MQOD_UI mqodUI;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Hello from MoreQOD!");
            Instance = this;
            mqodUI = new MQOD_UI();
            mqodUI.init();

            Hotkeys = MelonPreferences.CreateCategory("Hotkeys");
            sortingKeyEntry = Hotkeys.CreateEntry<KeyCode?>("sortingKey", KeyCode.S);
            toggleAutoSortingKeyEntry = Hotkeys.CreateEntry<KeyCode?>("toggleAutoSortingKey", KeyCode.P);
            toggleUIKeyEntry = Hotkeys.CreateEntry<KeyCode?>("toggleUIKeyEntry", KeyCode.U);
            minimapFullscreenEntry = Hotkeys.CreateEntry<KeyCode?>("minimapFullscreenEntry", KeyCode.Tab);
            minimapZoomOutEntry = Hotkeys.CreateEntry<KeyCode?>("minimapZoomOutEntry", KeyCode.Minus);
            minimapZoomInEntry = Hotkeys.CreateEntry<KeyCode?>("minimapZoomInEntry", KeyCode.Equals);
            Settings = MelonPreferences.CreateCategory("Settings");
            minimapZoomFunctionEntry =
                Settings.CreateEntry("minimapZoomFunctionEntry",
                    MQOD_UI.Minimap_ZoomFunction.HOLD);
            minimapTransparencyEntry = Settings.CreateEntry("minimapTransparencyEntry", 0.3f); 
            


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

            if (minimapFullscreen != null)
            {
                switch (MinimapZoomFunction)
                {
                    case MQOD_UI.Minimap_ZoomFunction.HOLD:
                        if (Input.GetKeyDown((KeyCode)minimapFullscreen)) BetterMinimapInst.fullscreenMinimap();
                        else if (Input.GetKeyUp((KeyCode)minimapFullscreen)) BetterMinimapInst.resetFullscreen();
                        break;
                    case MQOD_UI.Minimap_ZoomFunction.TOGGLE:
                        if (Input.GetKeyDown((KeyCode)minimapFullscreen))
                        {
                            if (!BetterMinimapInst.zoomedIn) BetterMinimapInst.fullscreenMinimap();
                            else BetterMinimapInst.resetFullscreen();
                        }

                        break;
                }
            }

            if (minimapZoomOut != null && Input.GetKeyDown((KeyCode)minimapZoomOut)) BetterMinimapInst.zoomOut();
            if (minimapZoomIn != null && Input.GetKeyDown((KeyCode)minimapZoomIn)) BetterMinimapInst.zoomIn();

            if (toggleAutoSortingKey != null && Input.GetKeyDown((KeyCode)toggleAutoSortingKey)) // middle click or S 
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
                }
            }


            if (toggleUIKey != null && mqodUI.initialized && !mqodUI.HotkeyPanel.toggleUITimer.Enabled &&
                Input.GetKeyDown((KeyCode)toggleUIKey))
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