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
        private readonly HashSet<KeyCode> activeKeybinds = new();
        private readonly List<MelonPreferences_Entry> Entries = new();
        private readonly MelonPreferences_Category Hotkeys = MelonPreferences.CreateCategory("Hotkeys");
        private readonly MelonPreferences_Category Settings = MelonPreferences.CreateCategory("Settings");

        public PreferencesManager()
        {
            TomletMain.RegisterMapper(ordering =>
            {
                TomlArray tomlArray = new();
                foreach (Sort.Category category in ordering) tomlArray.Add(category);

                return tomlArray;
            }, value =>
            {
                if (value is not TomlArray tomlArray)
                    throw new TomlTypeMismatchException(typeof(TomlArray), value.GetType(), typeof(Sort.Ordering));

                if (tomlArray.Count > (int)Sort.Category.NULL)
                {
                    MelonLogger.Warning("Sort Ordering parsing failed! Resorting to default.");
                    return Sort.Ordering.DEFAULT;
                }

                Sort.Ordering ordering = new();
                ordering.AddRange(tomlArray.Select(tomlValue =>
                    (Sort.Category)Enum.Parse(typeof(Sort.Category), tomlValue.StringValue)));
                return ordering;
            });
        }

        public MelonPreferences_Entry<T> addSettingsEntry<T>(string identifier, T default_value)
        {
            MelonPreferences_Entry<T> entry = Settings.CreateEntry(identifier, default_value);
            Entries.Add(entry);
            return entry;
        }

        public MelonPreferences_Entry<KeyCode?> addHotkeyEntry(string identifier, KeyCode? default_value = null)
        {
            MelonPreferences_Entry<KeyCode?> entry = Hotkeys.CreateEntry(identifier, default_value);
            if (default_value != null) activeKeybinds.Add((KeyCode)default_value);

            bool flag = false;
            entry.OnEntryValueChanged.Subscribe((oldKeyCode, newKeyCode) =>
            {
                MelonLogger.Msg($"{identifier} {oldKeyCode}=>{newKeyCode}");
                if (newKeyCode == null) return;
                if (newKeyCode == KeyCode.Escape)
                {
                    if (oldKeyCode != null) activeKeybinds.Remove((KeyCode)oldKeyCode);
                    entry.Value = null;
                    return;
                }

                if (activeKeybinds.Add((KeyCode)newKeyCode))
                {
                    if (oldKeyCode != null) activeKeybinds.Remove((KeyCode)oldKeyCode);
                }
                else if (!flag)
                {
                    flag = true;
                    entry.Value = oldKeyCode;
                    flag = false;
                }
                else
                {
                    activeKeybinds.Add((KeyCode)newKeyCode);
                }
            });
            Entries.Add(entry);
            return entry;
        }
    }
}