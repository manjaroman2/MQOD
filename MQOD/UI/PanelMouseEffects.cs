using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelMouseEffects : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<bool> Toggle;
        public readonly MelonPreferences_Entry<KeyCode?> ToggleHotkey;

        public PanelMouseEffects(UIBaseMQOD owner) : base(owner)
        {
            ToggleHotkey = prefManager.addHotkeyEntry("mouseEffectsToggleHotkey");
            Toggle = prefManager.addSettingsEntry("mouseEffectsToggle", false);
        }

        public override string Name => "MQOD - Mouse Effects";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        public void toggle()
        {
            Toggle.Value = !Toggle.Value;
            MQOD.Instance.MouseEffectsInst.setState(Toggle.Value);
        }

        protected override void LateConstructUI()
        {
            createHotkey("Toggle", ToggleHotkey);
            base.LateConstructUI();
        }
    }
}