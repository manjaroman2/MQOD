using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PanelMouseEffects : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<bool> ToggleEntry;
        public readonly MelonPreferences_Entry<KeyCode?> ToggleHotkey;

        public PanelMouseEffects(UIBaseMQOD owner) : base(owner)
        {
            ToggleHotkey = prefManager.addHotkeyEntry("mouseEffectsToggleHotkey");
            ToggleEntry = prefManager.addSettingsEntry("mouseEffectsToggle", false);
        }

        public override string Name => "MQOD - Mouse Effects";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        public void toggle()
        {
            ToggleEntry.Value = !ToggleEntry.Value;
            MQOD.Instance.MouseEffectsInst.setState(ToggleEntry.Value);
        }

        protected override void LateConstructUI()
        {
            createHotkey("Toggle", ToggleHotkey);
            base.LateConstructUI();
        }
    }
}