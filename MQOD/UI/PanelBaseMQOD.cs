using System;
using System.Timers;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace MQOD
{
    public abstract class PanelBaseMQOD : PanelBase
    {
        protected int fontSize;
        protected int rowCount;

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

            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, childControlWidth: true,
                childControlHeight: true, forceWidth: false, forceHeight: false,
                childAlignment: TextAnchor.UpperCenter,
                spacing: 5, padLeft: 20, padRight: 20, padTop: 0, padBottom: 20);
        }

        protected GameObject CreateRow()
        {
            rowCount += 1;
            return UIFactory.CreateHorizontalGroup(ContentRoot, $"Row{rowCount}", false, false, true, true, 20,
                bgColor: new Color(1f, 1f, 1f, 0.0f));
        }

        protected void createHotkey(string hotkeyLabel,
            Func<KeyCode?> keyCodeGetter,
            Action<KeyCode?> keyCodeSetter, out Text text, Timer keyTimout = null)
        {
            text = createHotkey(hotkeyLabel, keyCodeGetter, keyCodeSetter, keyTimout);
        }

        protected Text createHotkey(string hotkeyLabel,
            Func<KeyCode?> keyCodeGetter,
            Action<KeyCode?> keyCodeSetter, Timer keyTimout = null)
        {
            GameObject row = CreateRow();
            Text HotkeyLabel = UIFactory.CreateLabel(row, hotkeyLabel, hotkeyLabel);
            HotkeyLabel.fontSize = fontSize;
            HotkeyLabel.color = Color.green;
            UIFactory.SetLayoutElement(HotkeyLabel.gameObject, 25, 25, 1);
            ButtonRef Hotkey = UIFactory.CreateButton(row, $"{hotkeyLabel}Hotkey", keyCodeGetter().ToString());
            Hotkey.Component.GetComponentInChildren<Text>().fontSize = fontSize;
            Hotkey.Component.navigation = new Navigation
            {
                mode = Navigation.Mode.None
            };
            UIFactory.SetLayoutElement(Hotkey.GameObject, 100, 25, 0);
            Hotkey.OnClick += () =>
            {
                InputManager.BeginRebind(OnSelection, OnFinished);
                keyCodeSetter(null);
                Hotkey.ButtonText.text = "<Press any key>";
            };
            return HotkeyLabel;

            void OnSelection(KeyCode pressed)
            {
                if (keyTimout != null)
                {
                    keyTimout.AutoReset = false;
                    keyTimout.Enabled = true;
                }

                InputManager.EndRebind();
            }

            void OnFinished(KeyCode? bound)
            {
                keyCodeSetter(bound);
                Hotkey.ButtonText.text = keyCodeGetter().ToString();
            }
        }

        protected void createSwitch(string switchLabel, Color colorState1, Color colorState2,
            Func<bool> stateGetter, Action stateToggle, Func<string> stateString)
        {
            GameObject row = CreateRow();
            Text SwitchLabel = UIFactory.CreateLabel(row, switchLabel, switchLabel);
            SwitchLabel.fontSize = fontSize;
            SwitchLabel.color = stateGetter() ? colorState1 : colorState2;
            UIFactory.SetLayoutElement(SwitchLabel.gameObject, 25, 25, 1);
            ButtonRef Switch = UIFactory.CreateButton(row, $"{switchLabel}Switch", stateString());
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

        protected void createPanelSwitch<T>(string switchLabel, T panel, Color? colorState1 = null,
            Color? colorState2 = null)
            where T : PanelBase
        {
            colorState1 ??= Color.yellow;
            colorState2 ??= Color.gray;
            MelonPreferences_Entry<bool> panelExpandedEntry =
                MQOD.Instance.preferencesManager.addSettingsEntry(string.Join("", switchLabel.Split(' ')) + "Entry",
                    false);
            createSwitch(switchLabel, (Color)colorState1, (Color)colorState2, () => panelExpandedEntry.Value,
                () =>
                {
                    panelExpandedEntry.Value = !panelExpandedEntry.Value;
                    panel.Enabled = panelExpandedEntry.Value;
                    Vector3 vec3 = MQOD.Instance.UI.Main.Rect.localPosition;
                    panel.Rect.localPosition =
                        new Vector3(vec3.x + MQOD.Instance.UI.Main.Rect.sizeDelta.x, vec3.y, vec3.z);
                },
                () => panelExpandedEntry.Value ? "Expanded" : "Hidden");
        }

        protected void createSlider(string label, float minValue, float maxValue,
            Action<float> onValueChanged,
            Func<float> valueGetter)
        {
            GameObject row = CreateRow();

            Text Label = UIFactory.CreateLabel(row, label, $"{label}: {valueGetter():0.00}");
            Label.fontSize = fontSize;
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
            // slider.colors = new ColorBlock();
            slider.navigation = new Navigation
            {
                mode = Navigation.Mode.None
            };
        }

        public static ColorBlock generateColorBlock(Color color)
        {
            ColorBlock colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = color;
            colorBlock.highlightedColor = color;
            colorBlock.pressedColor = color;
            colorBlock.selectedColor = color;
            return colorBlock;
        }

        public static Color FlatToColor(float f)
        {
            int flat = Mathf.RoundToInt(f);

            int[] c = new int[6];
            int remainder = flat % 256;
            int n = flat / 256;
            int i = 0;
            while (i < n)
            {
                c[i] = 255;
                i++;
            }

            c[i] = remainder;

            byte r = (byte)(255 + c[4] - c[1]);
            byte g = (byte)(0 + c[0] - c[3]);
            byte b = (byte)(0 + c[2] - c[5]);
            return new Color32(r, g, b, 255);
        }

        protected void createColorSlider(string label,
            Action<float> onValueChanged,
            Func<float> valueGetter)
        {
            GameObject row = CreateRow();

            Text Label = UIFactory.CreateLabel(row, label, $"{label}");
            Label.fontSize = fontSize;
            UIFactory.SetLayoutElement(Label.gameObject, 75, 25, 0);
            GameObject GObj_Slider = CreateSlider(row, $"{label}Slider", out Slider slider, out Image image1,
                out Image image2);

            void applyColor(Color color)
            {
                Label.color = color;
                slider.colors = generateColorBlock(color);
                image1.color = color;
                image2.color = color;
            }

            UIFactory.SetLayoutElement(GObj_Slider, 400, 25, 1);
            slider.minValue = 0.0f;
            slider.maxValue = 256f * 6f;
            slider.value = valueGetter();
            slider.onValueChanged.AddListener(f =>
            {
                Label.text = $"{label}";
                applyColor(FlatToColor(f));
                onValueChanged(f);
            });
            applyColor(FlatToColor(0.0f));
            slider.navigation = new Navigation
            {
                mode = Navigation.Mode.None
            };
        }

        protected void createTextEntry(string label)
        {
            GameObject row = CreateRow();
            Text Label = UIFactory.CreateLabel(row, label, label);
            Label.fontSize = fontSize;
            UIFactory.SetLayoutElement(Label.gameObject, 25, 25, 1);
        }


        protected GameObject CreateSlider(GameObject parent, string name, out Slider slider, out Image image1,
            out Image image2)
        {
            GameObject uiObject1 = UIFactory.CreateUIObject(name, parent, new Vector2(25f, 25f));
            GameObject uiObject2 = UIFactory.CreateUIObject("Background", uiObject1);
            GameObject uiObject3 = UIFactory.CreateUIObject("Fill Area", uiObject1);
            GameObject uiObject4 = UIFactory.CreateUIObject("Fill", uiObject3);
            GameObject uiObject5 = UIFactory.CreateUIObject("Handle Slide Area", uiObject1);
            GameObject uiObject6 = UIFactory.CreateUIObject("Handle", uiObject5);
            image1 = uiObject2.AddComponent<Image>();
            image1.type = Image.Type.Sliced;
            RectTransform component1 = uiObject2.GetComponent<RectTransform>();
            component1.anchorMin = new Vector2(0.0f, 0.25f);
            component1.anchorMax = new Vector2(1f, 0.75f);
            component1.sizeDelta = new Vector2(0.0f, 0.0f);
            RectTransform component2 = uiObject3.GetComponent<RectTransform>();
            component2.anchorMin = new Vector2(0.0f, 0.25f);
            component2.anchorMax = new Vector2(1f, 0.75f);
            component2.anchoredPosition = new Vector2(-5f, 0.0f);
            component2.sizeDelta = new Vector2(-20f, 0.0f);
            image2 = uiObject4.AddComponent<Image>();
            image2.type = Image.Type.Sliced;
            uiObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 0.0f);
            RectTransform component3 = uiObject5.GetComponent<RectTransform>();
            component3.sizeDelta = new Vector2(-20f, 0.0f);
            component3.anchorMin = new Vector2(0.0f, 0.0f);
            component3.anchorMax = new Vector2(1f, 1f);
            Image image3 = uiObject6.AddComponent<Image>();
            image3.color = new Color(0.5f, 0.5f, 1f, 1f);
            uiObject6.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 0.0f);
            slider = uiObject1.AddComponent<Slider>();
            slider.fillRect = uiObject4.GetComponent<RectTransform>();
            slider.handleRect = uiObject6.GetComponent<RectTransform>();
            slider.targetGraphic = (Graphic)image3;
            slider.direction = Slider.Direction.LeftToRight;
            // RuntimeHelper.Instance.Internal_SetColorBlock((Selectable) slider, new Color?(new Color(0.4f, 0.4f, 0.4f)), new Color?(new Color(0.55f, 0.55f, 0.55f)), new Color?(new Color(0.3f, 0.3f, 0.3f)));
            return uiObject1;
        }
    }
}