using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;
using UnityEngine;

namespace MQOD
{
    public class PreferencesManager
    {
        public MelonPreferences_Entry<Sort.Ordering> customSortOrderingEntry;
        public MelonPreferences_Entry<float> widthModifier;
        public MelonPreferences_Entry<float> gemRadiusColorFloat;
        public MelonPreferences_Category Hotkeys;
        public MelonPreferences_Entry<float> minimapTransparencyEntry;
        public MelonPreferences_Entry<bool> minimapZoomFunctionEntry;
        public MelonPreferences_Category Settings;
        public MelonPreferences_Entry<KeyCode?> minimapFullscreenKeyEntry;
        public MelonPreferences_Entry<KeyCode?> minimapZoomInKeyEntry;
        public MelonPreferences_Entry<KeyCode?> minimapZoomOutKeyEntry;
        public MelonPreferences_Entry<KeyCode?> sortingKeyEntry;
        public MelonPreferences_Entry<KeyCode?> toggleAutoSortingKeyEntry;
        public MelonPreferences_Entry<KeyCode?> toggleUIKeyEntry;
        public MelonPreferences_Entry<KeyCode?> cameraZoomKeyEntry;
        public MelonPreferences_Entry<KeyCode?> gemRadiusVisualizerToggleKeyEntry;
         

        private readonly List<MelonPreferences_Entry> entries = new();

        public void init()
        {
            MelonLogger.Msg("PreferencesManager Init");
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
            widthModifier = Settings.CreateEntry("gemVisualizerWidthModifierEntry", 0.5f);
            TomletMain.RegisterMapper(ordering =>
            {
                TomlArray tomlArray = new();
                foreach (Sort.Category category in ordering)
                {
                    tomlArray.Add(category);
                }

                return tomlArray;
            }, value =>
            {
                if (value is not TomlArray tomlArray)
                    throw new TomlTypeMismatchException(typeof(TomlArray), value.GetType(), typeof(Sort.Ordering));

                Sort.Ordering ordering = new(tomlArray.Count);
                ordering.AddRange(tomlArray.Select(tomlValue =>
                    (Sort.Category)Enum.Parse(typeof(Sort.Category), tomlValue.StringValue)));

                return ordering;
            });
            customSortOrderingEntry = Settings.CreateEntry("customSortOrderingEntry", new Sort.Ordering
            {
                Sort.Category.UNIQUENESS, Sort.Category.RARITY, Sort.Category.TIER, Sort.Category.TYPE
            });
            gemRadiusColorFloat = Settings.CreateEntry("gemRadiusColorFloat", 0.0f);
            cameraZoomKeyEntry = Hotkeys.CreateEntry<KeyCode?>("cameraZoomKeyEntry", KeyCode.Semicolon);
            gemRadiusVisualizerToggleKeyEntry =
                Hotkeys.CreateEntry<KeyCode?>("gemRadiusVisualizerToggleKeyEntry", KeyCode.Quote);

        }

        public MelonPreferences_Entry<T> addSettingsEntry<T>(string identifier, T default_value)
        {
            MelonPreferences_Entry<T> entry = Settings.CreateEntry(identifier, default_value);
            entries.Add(entry);
            return entry;
        }
        
        
    }
}