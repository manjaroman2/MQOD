using System;
using System.Collections.Generic;
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
        private const float startupDelay = 1f;
        public CustomHotkeyPanel HotkeyPanel;
        public bool initialized;
        public CustomSortPanel SortPanel;
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
                Disable_EventSystem_Override = false, // or null
                Force_Unlock_Mouse = true, // or null
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
            HotkeyPanel = new CustomHotkeyPanel(UIBase);
            SortPanel = new CustomSortPanel(UIBase)
            {
                Enabled = MQOD.Instance.mqodUI.customSortSettingsExpanded
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

        public abstract class MQOD_PanelBase : PanelBase
        {
            protected int fontSize;

            protected MQOD_PanelBase(UIBase owner) : base(owner)
            {
            }

            protected override void ConstructPanelContent()
            {
                fontSize = MQOD.Instance.mqodUI.fontSize;
                Text TitleBarText = ContentRoot.GetComponentInChildren<Text>();
                if (TitleBarText != null)
                {
                    TitleBarText.fontSize = fontSize;
                    TitleBar.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = true;
                }
            }


            protected Text createHotkey(int i, string hotkeyLabel,
                Func<KeyCode?> keyCodeGetter,
                Action<KeyCode?> keyCodeSetter, Timer keyTimout = null)
            {
                GameObject row1 = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text HotkeyLabel = UIFactory.CreateLabel(row1, hotkeyLabel, hotkeyLabel);
                HotkeyLabel.fontSize = fontSize;
                HotkeyLabel.color = Color.green;
                UIFactory.SetLayoutElement(HotkeyLabel.gameObject, 25, 25, 1);
                ButtonRef Hotkey = UIFactory.CreateButton(row1, $"{hotkeyLabel}Hotkey", keyCodeGetter().ToString());
                Hotkey.Component.GetComponentInChildren<Text>().fontSize = fontSize;
                Hotkey.Component.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
                UIFactory.SetLayoutElement(Hotkey.GameObject, 100, 25, 0);
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


            protected void createSwitch(int i, string switchLabel, Color colorState1, Color colorState2,
                Func<bool> stateGetter, Action stateToggle, Func<string> stateString)
            {
                GameObject row1 = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text SwitchLabel = UIFactory.CreateLabel(row1, switchLabel, switchLabel);
                SwitchLabel.fontSize = fontSize;
                SwitchLabel.color = stateGetter() ? colorState1 : colorState2;
                UIFactory.SetLayoutElement(SwitchLabel.gameObject, 25, 25, 1);
                ButtonRef Switch = UIFactory.CreateButton(row1, $"{switchLabel}Switch", stateString());
                UIFactory.SetLayoutElement(Switch.GameObject, 100, 25, 0);
                Switch.Component.GetComponentInChildren<Text>().fontSize = fontSize;
                Switch.Component.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
                Switch.OnClick += () =>
                {
                    stateToggle();
                    Switch.ButtonText.text = stateString();
                    SwitchLabel.color = stateGetter() ? colorState1 : colorState2;
                };
            }

            protected void createSlider(int i, string label, float minValue, float maxValue,
                Action<float> onValueChanged,
                Func<float> valueGetter)
            {
                GameObject row = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text Label = UIFactory.CreateLabel(row, label, $"{label}: {valueGetter():0.00}");
                Label.fontSize = fontSize;
                // Label.color = colorState1;
                UIFactory.SetLayoutElement(Label.gameObject, 25, 25, 1);
                GameObject GObj_Slider = UIFactory.CreateSlider(row, $"{label}Slider", out Slider slider);
                UIFactory.SetLayoutElement(GObj_Slider, 100, 25, 0);
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                slider.value = valueGetter();
                slider.onValueChanged.AddListener(f =>
                {
                    Label.text = $"{label}: {valueGetter():0.00}";
                    onValueChanged(f);
                });
                slider.navigation = new Navigation
                {
                    mode = Navigation.Mode.None
                };
            }

            protected void createTextEntry(int i, string label)
            {
                GameObject row = UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{i}", false, false, true, true, 20,
                    bgColor: new Color(1f, 1f, 1f, 0.0f));

                Text Label = UIFactory.CreateLabel(row, label, label);
                Label.fontSize = fontSize;
                // Label.color = colorState1;
                UIFactory.SetLayoutElement(Label.gameObject, 25, 25, 1);
            }
        }

        public class CustomHotkeyPanel : MQOD_PanelBase
        {
            public readonly Timer toggleUITimer = new(1000);
            public Text toggleAutoSortingLabel;

            public CustomHotkeyPanel(UIBase owner) : base(owner)
            {
            }

            public override string Name => "MQOD - Hotkeys";
            public override int MinWidth => 300;
            public override int MinHeight => 500;
            public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
            public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
            public override bool CanDragAndResize => true;

            protected override void ConstructPanelContent()
            {
                base.ConstructPanelContent();
                UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, childControlWidth: true,
                    childControlHeight: true, forceHeight: false, forceWidth: false, padBottom: 20, padLeft: 20,
                    padRight: 20, spacing: 5);
                createHotkey(0, "UIToggle", () => MQOD.Instance.mqodUI.toggleUIKey,
                    code => MQOD.Instance.mqodUI.toggleUIKey = code,
                    toggleUITimer);
                createHotkey(1, "Sorting", () => MQOD.Instance.mqodUI.sortingKey,
                    code => MQOD.Instance.mqodUI.sortingKey = code);
                toggleAutoSortingLabel = createHotkey(2, "toggleAutoSorting [enabled]",
                    () => MQOD.Instance.mqodUI.toggleAutoSortingKey,
                    code => MQOD.Instance.mqodUI.toggleAutoSortingKey = code);
                createSwitch(3, "Custom Sort Settings", Color.yellow, Color.gray,
                    () => MQOD.Instance.mqodUI.customSortSettingsExpanded,
                    () =>
                    {
                        MQOD.Instance.mqodUI.customSortSettingsExpanded =
                            !MQOD.Instance.mqodUI.customSortSettingsExpanded;
                        MQOD.Instance.mqodUI.SortPanel.Enabled = MQOD.Instance.mqodUI.customSortSettingsExpanded;
                    },
                    () => MQOD.Instance.mqodUI.customSortSettingsExpanded ? "Expanded" : "Hidden");
                createHotkey(4, "Fullscreen Minimap", () => MQOD.Instance.mqodUI.minimapFullscreenKey,
                    code => MQOD.Instance.mqodUI.minimapFullscreenKey = code);
                createHotkey(5, "minimapZoomOut", () => MQOD.Instance.mqodUI.minimapZoomOutKey,
                    code => MQOD.Instance.mqodUI.minimapZoomOutKey = code);
                createHotkey(6, "minimapZoomIn", () => MQOD.Instance.mqodUI.minimapZoomInKey,
                    code => MQOD.Instance.mqodUI.minimapZoomInKey = code);

                createSwitch(7, "Minimap Function", Color.gray, Color.gray,
                    () => MQOD.Instance.mqodUI.MinimapZoomFunction,
                    () => MQOD.Instance.mqodUI.MinimapZoomFunction = !MQOD.Instance.mqodUI.MinimapZoomFunction,
                    () => MQOD.Instance.mqodUI.MinimapZoomFunction ? "Toggle" : "Hold");

                createSlider(8, "Minimap opacity", 0.01f, 1.00f, f =>
                {
                    MQOD.Instance.mqodUI.minimapTransparency = f;
                    if (MQOD.Instance.BetterMinimapInst.initialized && MQOD.Instance.BetterMinimapInst.zoomedIn)
                    {
                        Color boundsImage_color = MQOD.Instance.BetterMinimapInst.boundsImage_color;
                        MQOD.Instance.BetterMinimapInst.boundsImage.color = new Color(boundsImage_color.r,
                            boundsImage_color.g, boundsImage_color.b, MQOD.Instance.mqodUI.minimapTransparency);
                    }
                }, () => MQOD.Instance.mqodUI.minimapTransparency);
            }
        }

        public class CustomSortPanel : MQOD_PanelBase
        {
            public CustomSortPanel(UIBase owner) : base(owner)
            {
            }

            public Sort.Ordering SortOrdering
            {
                get => MQOD.Instance.preferencesManager.customSortOrderingEntry.Value;
                set => MQOD.Instance.preferencesManager.customSortOrderingEntry.Value = value;
            }

            public override string Name => "MQOD - Custom Sort Settings";
            public override int MinWidth => 300;
            public override int MinHeight => 300;
            public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
            public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
            public override bool CanDragAndResize => true;

            protected override void ConstructPanelContent()
            {
                base.ConstructPanelContent();
                UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, childControlWidth: true,
                    childControlHeight: true, forceWidth: false, forceHeight: false,
                    childAlignment: TextAnchor.UpperCenter,
                    spacing: 5, padLeft: 20, padRight: 20, padTop: 0, padBottom: 20);

                List<Text> texts = new();
                Dictionary<Text, Sort.Category> CategoryIndex = new();

                foreach (Sort.Category category in SortOrdering)
                {
                    Text text = UIFactory.CreateLabel(ContentRoot, category.GetString(), category.GetString(),
                        fontSize: fontSize);
                    DragController daDragController = text.gameObject.AddComponent<DragController>();
                    daDragController.currentTransform = text.rectTransform;
                    UIFactory.SetLayoutElement(text.gameObject, 25, 25, 1);

                    CategoryIndex[text] = category;
                    texts.Add(text);

                    daDragController.callback = () =>
                    {
                        Sort.Ordering oldOrdering = SortOrdering;
                        Sort.Ordering newOrdering = new(oldOrdering.Count);
                        foreach (Text textIter in texts)
                        {
                            int oldIdx = oldOrdering.IndexOf(CategoryIndex[textIter]);
                            int newIdx = textIter.transform.GetSiblingIndex() - 1;
                            // MelonLogger.Msg($"{oldIdx} {newIdx}");
                            newOrdering[newIdx] = oldOrdering[oldIdx];
                        }

                        // MelonLogger.Msg("C");
                        SortOrdering = newOrdering;
                        // MelonLogger.Msg("D");
                    };
                }
            }

            public void loadVariables(Sort.Ordering entries)
            {
                SortOrdering = entries;
            }
        }
    }
}