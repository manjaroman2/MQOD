using System;
using System.IO;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;
using Timer = System.Timers.Timer;

namespace MQOD
{
    public class MQOD_UI
    {
        public UIBase UIBase;
        public MyPanel HotkeyPanel;
        public bool initialized;

        private const float startupDelay = 1f;

        public enum Minimap_ZoomFunction
        {
            HOLD,
            TOGGLE
        }

        public void init()
        {
            UniverseLibConfig config = new()
            {
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
                Unhollowed_Modules_Folder = Path.Combine(Path.GetDirectoryName(MelonHandler.ModsDirectory)!,
                    Path.Combine("MelonLoader", "Managed"))
            };

            Universe.Init(startupDelay, OnInitialized, LogHandler, config);
        }

        public class MyPanel : PanelBase
        {
            public MyPanel(UIBase owner) : base(owner)
            {
            }

            public override string Name => "MQOD - Hotkeys";
            public override int MinWidth => 100;
            public override int MinHeight => 200;
            public override Vector2 DefaultAnchorMin => new(0.00f, 0.00f);
            public override Vector2 DefaultAnchorMax => new(0.20f, 0.30f);
            public override bool CanDragAndResize => true;
            public int fontSize = 18;

            public readonly Timer toggleUITimer = new(1000);
            public Text toggleAutoSortingLabel;


            protected override void ConstructPanelContent()
            {
                UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot);
                createHotkey(0, "UIToggle", () => MQOD.Instance.toggleUIKey, code => MQOD.Instance.toggleUIKey = code,
                    toggleUITimer);
                createHotkey(1, "Sorting", () => MQOD.Instance.sortingKey, code => MQOD.Instance.sortingKey = code);
                toggleAutoSortingLabel = createHotkey(2, "toggleAutoSorting [enabled]",
                    () => MQOD.Instance.toggleAutoSortingKey, code => MQOD.Instance.toggleAutoSortingKey = code);
                createHotkey(3, "Fullscreen Minimap", () => MQOD.Instance.minimapFullscreen,
                    code => MQOD.Instance.minimapFullscreen = code);
                createHotkey(4, "minimapZoomOut", () => MQOD.Instance.minimapZoomOut,
                    code => MQOD.Instance.minimapZoomOut = code);
                createHotkey(5, "minimapZoomIn", () => MQOD.Instance.minimapZoomIn,
                    code => MQOD.Instance.minimapZoomIn = code);

                createSwitch(6, "Minimap Hold/Toggle", Color.gray, Color.gray, () =>
                    {
                        return MQOD.Instance.MinimapZoomFunction switch
                        {
                            Minimap_ZoomFunction.HOLD => "Hold",
                            Minimap_ZoomFunction.TOGGLE => "Toggle",
                            _ => null
                        };
                    }, () =>
                    {
                        MQOD.Instance.MinimapZoomFunction = MQOD.Instance.MinimapZoomFunction switch
                        {
                            Minimap_ZoomFunction.HOLD => Minimap_ZoomFunction.TOGGLE,
                            Minimap_ZoomFunction.TOGGLE => Minimap_ZoomFunction.HOLD,
                            _ => MQOD.Instance.MinimapZoomFunction
                        };
                    }
                );

                createSlider(7, "Minimap opacity", 0.01f, 1.00f, f =>
                {
                    MQOD.Instance.minimapTransparency = f;
                    if (MQOD.Instance.BetterMinimapInst.initialized && MQOD.Instance.BetterMinimapInst.zoomedIn)
                    {  
                        Color boundsImage_color = MQOD.Instance.BetterMinimapInst.boundsImage_color;
                        MQOD.Instance.BetterMinimapInst.boundsImage.color = new Color(boundsImage_color.r,
                            boundsImage_color.g, boundsImage_color.b, MQOD.Instance.minimapTransparency);   
                    }
                }, () => MQOD.Instance.minimapTransparency);
            }

            private Text createHotkey(int i, string hotkeyLabel, Func<KeyCode?> keyCodeGetter,
                Action<KeyCode?> keyCodeSetter, Timer keyTimout = null)
            {
                GameObject row1 = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text HotkeyLabel = UIFactory.CreateLabel(row1, hotkeyLabel, hotkeyLabel);
                HotkeyLabel.fontSize = fontSize;
                HotkeyLabel.color = Color.green;
                UIFactory.SetLayoutElement(HotkeyLabel.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 1);
                ButtonRef Hotkey = UIFactory.CreateButton(row1, $"{hotkeyLabel}Hotkey", keyCodeGetter().ToString());
                Hotkey.Component.GetComponentInChildren<Text>().fontSize = fontSize;
                Hotkey.Component.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
                UIFactory.SetLayoutElement(Hotkey.GameObject, minWidth: 75, minHeight: 25, flexibleWidth: 0);
                Hotkey.OnClick += () =>
                {
                    MelonLogger.Msg("Clicked!");
                    InputManager.BeginRebind(OnSelection, OnFinished);
                    keyCodeSetter(null);
                    Hotkey.ButtonText.text = "<Press any key>";
                };
                return HotkeyLabel;

                void OnSelection(KeyCode pressed)
                {
                    MelonLogger.Msg("OnSelection");
                    if (keyTimout != null)
                    {
                        keyTimout.AutoReset = false;
                        keyTimout.Enabled = true;
                    }

                    InputManager.EndRebind();
                }

                void OnFinished(KeyCode? bound)
                {
                    MelonLogger.Msg("OnFinished");
                    keyCodeSetter(bound);
                    Hotkey.ButtonText.text = keyCodeGetter().ToString();
                }
            }

            private void createSwitch(int i, string switchLabel, Color colorState1, Color colorState2,
                Func<string> stateGetter, Action stateToggle)
            {
                GameObject row1 = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text HotkeyLabel = UIFactory.CreateLabel(row1, switchLabel, switchLabel);
                HotkeyLabel.fontSize = fontSize;
                HotkeyLabel.color = colorState1;
                UIFactory.SetLayoutElement(HotkeyLabel.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 1);
                ButtonRef Switch = UIFactory.CreateButton(row1, $"{switchLabel}Switch", stateGetter());
                UIFactory.SetLayoutElement(Switch.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 0);
                Switch.Component.GetComponentInChildren<Text>().fontSize = fontSize;
                Switch.Component.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
                Switch.OnClick += () =>
                {
                    stateToggle();
                    Switch.ButtonText.text = stateGetter();
                };
            }

            private void createSlider(int i, string label, float minValue, float maxValue, Action<float> onValueChanged, Func<float> valueGetter)
            {
                GameObject row = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text Label = UIFactory.CreateLabel(row, label, $"{label}: {valueGetter():0.00}");
                Label.fontSize = fontSize;
                // Label.color = colorState1;
                UIFactory.SetLayoutElement(Label.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 1);
                GameObject GObj_Slider = UIFactory.CreateSlider(row, $"{label}Slider", out Slider slider);
                UIFactory.SetLayoutElement(GObj_Slider, minWidth: 100, minHeight: 25, flexibleWidth: 0);
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                slider.value = valueGetter();
                slider.onValueChanged.AddListener((f) =>
                {
                    Label.text = $"{label}: {valueGetter():0.00}";
                    onValueChanged(f);
                });
                slider.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
            }

            private void createTextEntry(int i, string label)
            {
                
                GameObject row = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text Label = UIFactory.CreateLabel(row, label, label);
                Label.fontSize = fontSize;
                // Label.color = colorState1;
                UIFactory.SetLayoutElement(Label.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 1);
            }
        }


        private void UiUpdate()
        {
        }

        private void OnInitialized()
        {
            UIBase = UniversalUI.RegisterUI("mj.MQOD", UiUpdate);
            HotkeyPanel = new MyPanel(UIBase);
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