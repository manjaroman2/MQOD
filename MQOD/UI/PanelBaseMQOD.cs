using System;
using System.Timers;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace MQOD
{
    public abstract class PanelBaseMQOD : PanelBase
    {
        protected int fontSize;

        protected PanelBaseMQOD(UIBase owner) : base(owner)
        {
        }

        protected override void ConstructPanelContent()
        {
            fontSize = MQOD.Instance.UI.fontSize;
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
}