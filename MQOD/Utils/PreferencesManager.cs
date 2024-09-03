using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class PreferencesManager
    {
        public MelonPreferences_Entry<Sort.Ordering> customSortOrderingEntry;
        public MelonPreferences_Entry<bool> customSortSettingsExpandedEntry;
        public MelonPreferences_Category Hotkeys;
        public MelonPreferences_Entry<KeyCode?> minimapFullscreenKeyEntry;
        public MelonPreferences_Entry<float> minimapTransparencyEntry;
        public MelonPreferences_Entry<bool> minimapZoomFunctionEntry;
        public MelonPreferences_Entry<KeyCode?> minimapZoomInKeyEntry;
        public MelonPreferences_Entry<KeyCode?> minimapZoomOutKeyEntry;
        public MelonPreferences_Category Settings;
        public MelonPreferences_Entry<KeyCode?> sortingKeyEntry;
        public MelonPreferences_Entry<KeyCode?> toggleAutoSortingKeyEntry;
        public MelonPreferences_Entry<KeyCode?> toggleUIKeyEntry;

        public void init()
        {
            Hotkeys = MelonPreferences.CreateCategory("Hotkeys");
            sortingKeyEntry = Hotkeys.CreateEntry<KeyCode?>("sortingKey", KeyCode.S);
            toggleAutoSortingKeyEntry = Hotkeys.CreateEntry<KeyCode?>("toggleAutoSortingKey", KeyCode.P);
            toggleUIKeyEntry = Hotkeys.CreateEntry<KeyCode?>("toggleUIKeyEntry", KeyCode.U);
            minimapFullscreenKeyEntry = Hotkeys.CreateEntry<KeyCode?>("minimapFullscreenEntry", KeyCode.Tab);
            minimapZoomOutKeyEntry = Hotkeys.CreateEntry<KeyCode?>("minimapZoomOutEntry", KeyCode.Minus);
            minimapZoomInKeyEntry = Hotkeys.CreateEntry<KeyCode?>("minimapZoomInEntry", KeyCode.Equals);
            Settings = MelonPreferences.CreateCategory("Settings");
            minimapZoomFunctionEntry =
                Settings.CreateEntry("minimapZoomFunctionEntry", false);
            minimapTransparencyEntry = Settings.CreateEntry("minimapTransparencyEntry", 0.3f);
            customSortSettingsExpandedEntry = Settings.CreateEntry("customSortSettingsExpandedEntry", false);
            customSortOrderingEntry = Settings.CreateEntry("customSortOrderingEntry", new Sort.Ordering
            {
                Sort.Category.UNIQUENESS, Sort.Category.RARITY, Sort.Category.TIER, Sort.Category.TYPE
            });
        }
    }
}