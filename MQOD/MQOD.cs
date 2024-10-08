﻿using System;
using System.Reflection;
using Claw.UserInterface.Screens;
using Death;
using Death.TimesRealm;
using Death.TimesRealm.UserInterface;
using Death.UserInterface;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MQOD
{
    public class MQOD : MelonMod
    {
        private static MQOD _Instance;

        public readonly AssetManager assetManager = new();
        private readonly FeatureManager featureManager = new();
        public readonly PreferencesManager preferencesManager = new();
        public BetterMinimap BetterMinimapInst;
        public CameraZoom CameraZoomInst;
        public GemRadiusVisualizer GemRadiusVisualizerInst;

        protected bool isRun;
        public MouseEffects MouseEffectsInst;
        public ScreenManager ScreenManager;
        public ShopReroll ShopRerollInst;

        public SortArmory SortArmoryInst;
        public SortItemGrid SortItemGridInst;
        public SortShop SortShopInst;
        public SortStash SortStashInst;
        public UI UIInst;

        public static MQOD Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                MelonLogger.Error("MQOD _Instance is null!");
                throw new NullReferenceException("MQOD _Instance is null!");
            }
        }

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("OnInitializeMelon");
            
            
            _Instance = this;

            assetManager.init();
            UIInst = new UI();
            UIInst.init();

            SortItemGridInst = featureManager.addFeature<SortItemGrid>();
            SortStashInst = featureManager.addFeature<SortStash>();
            SortShopInst = featureManager.addFeature<SortShop>();
            SortArmoryInst = featureManager.addFeature<SortArmory>();
            BetterMinimapInst = featureManager.addFeature<BetterMinimap>();
            GemRadiusVisualizerInst = featureManager.addFeature<GemRadiusVisualizer>();
            CameraZoomInst = featureManager.addFeature<CameraZoom>();
            ShopRerollInst = featureManager.addFeature<ShopReroll>();
            MouseEffectsInst = featureManager.addFeature<MouseEffects>();
#if DEBUG
            featureManager.addFeature<AutoStart>();
#endif
            featureManager.addHarmonyHooks();


            HarmonyHelper.Patch(typeof(Facade_Lobby), nameof(Facade_Lobby.Init), new[] { typeof(ILobbyGameState) },
                postfixClazz: typeof(MQOD), postfixMethod: nameof(Facade_Lobby__Init__Postfix));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "Scene_Core":
                    // MelonLogger.Warning("inputsystem: " + EventSystem.current.currentInputModule);
                    // MelonLogger.Msg(Game.NavigateAction.controls);
                    foreach (PropertyInfo propertyInfo in typeof(Controls.UIActions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (propertyInfo.PropertyType == typeof(InputAction) && propertyInfo.Name != "Scroll")
                        {
                            InputAction inputAction = (InputAction)propertyInfo.GetValue(Game.Controls.UI);
                            MelonLogger.Msg("inputAction: " + inputAction);
                            inputAction.performed += context =>
                            {
                                MelonLogger.Warning($"{context.action} performed!");
                            }; 
                            
                        }
                    }
                    // Game.Controls.UI.Get().asset.bindings.GetEnumerator().Current.
                    break;
                case "Scene_Run":
                    isRun = true;
                    break;
                case "Scene_RunGUI":
                    BetterMinimapInst.init();
                    break;
                case "Scene_TimesRealm":
                    isRun = false;
                    break;
                case "Scene_LobbyGUI":
                    break;
                case "Scene_Bootstrap":
                    // dGameManager = SceneManager.GetActiveScene().GetRootGameObjects()[0].GetComponent<GameManager>();
                    break;
            }

            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
        }

        public override void OnLateInitializeMelon()
        {
            MelonLogger.Msg("OnLateInitializeMelon");
        }


        public override void OnLateUpdate()
        {
            if (!UIInst.initialized)
            {
                return;
            }

            
            // if (MouseEffectsInst is { initialized: true })
            //     MouseEffectsInst.calcRotation(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            if (UIInst.keyReassignTimer.Enabled) return; // Handle keycodes

            // if (Input.GetKeyDown(KeyCode.L)) Player.Instance.Entity.Invulnerable.AddStack();

            if (UIInst.FeatureMinimap.minimapFullscreenKeyEntry.Value != null)
            {
                if (!UIInst.FeatureMinimap.minimapZoomFunctionEntry.Value)
                {
                    if (Input.GetKeyDown((KeyCode)UIInst.FeatureMinimap.minimapFullscreenKeyEntry.Value))
                        BetterMinimapInst.Fullscreen();
                    else if (Input.GetKeyUp((KeyCode)UIInst.FeatureMinimap.minimapFullscreenKeyEntry.Value))
                        BetterMinimapInst.unFullscreen();
                }
                else
                {
                    if (Input.GetKeyDown((KeyCode)UIInst.FeatureMinimap.minimapFullscreenKeyEntry.Value))
                    {
                        if (!BetterMinimapInst.IsFullscreen) BetterMinimapInst.Fullscreen();
                        else BetterMinimapInst.unFullscreen();
                    }
                }
            }

            if (UIInst.FeatureMinimap.minimapZoomOutKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureMinimap.minimapZoomOutKeyEntry.Value))
                BetterMinimapInst.zoomOut();
            if (UIInst.FeatureMinimap.minimapZoomInKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureMinimap.minimapZoomInKeyEntry.Value))
                BetterMinimapInst.zoomIn();

            if (UIInst.FeatureSort.toggleAutoSortingKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureSort.toggleAutoSortingKeyEntry.Value)) // middle click or S 
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

                if (UIInst.FeatureSort.toggleAutoSortingLabel != null)
                {
                    UIInst.FeatureSort.toggleAutoSortingLabel.color = color;
                    UIInst.FeatureSort.toggleAutoSortingLabel.text = $"toggleAutoSorting [{text}]";
                }

                MelonLogger.Msg($"Item grid sorting: {text}");
            }

            if (UIInst.FeatureSort.sortingKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureSort.sortingKeyEntry.Value) &&
                ScreenManager != null)
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

            if (UIInst.FeatureCamera.cameraZoomKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureCamera.cameraZoomKeyEntry.Value))
                CameraZoomInst.zoomOut();

            if (UIInst.FeatureGemVisualizer.gemRadiusVisualizerToggleKeyEntry.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureGemVisualizer.gemRadiusVisualizerToggleKeyEntry.Value))
                GemRadiusVisualizerInst.toggle();

            if (UIInst.FeatureMouseEffects.ToggleHotkey.Value != null &&
                Input.GetKeyDown((KeyCode)UIInst.FeatureMouseEffects.ToggleHotkey.Value))
                UIInst.FeatureMouseEffects.toggle();

            if (UIInst.Main.toggleUIKeyEntry.Value != null && UIInst.initialized && !UIInst.keyReassignTimer.Enabled &&
                Input.GetKeyDown((KeyCode)UIInst.Main.toggleUIKeyEntry.Value))
            {
                UIInst.UIBase.Enabled = !UIInst.UIBase.Enabled;
                if (UIInst.UIBase.Enabled) Game.Pause();
                else Game.Resume();
            }
            // MelonLogger.Msg("Toggle UI");
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