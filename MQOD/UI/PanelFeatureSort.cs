using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureSort : PanelBaseMQOD
    {
        public readonly MelonPreferences_Entry<Sort.Ordering> customSortOrderingEntry;
        public readonly MelonPreferences_Entry<KeyCode?> sortingKeyEntry;
        public readonly MelonPreferences_Entry<KeyCode?> toggleAutoSortingKeyEntry;
        public Text toggleAutoSortingLabel;

        public PanelFeatureSort(UIBaseMQOD owner) : base(owner)
        {
            sortingKeyEntry = prefManager.addHotkeyEntry("sortingKey");
            toggleAutoSortingKeyEntry = prefManager.addHotkeyEntry("toggleAutoSortingKey");
            customSortOrderingEntry = prefManager.addSettingsEntry("customSortOrderingEntry", Sort.Ordering.DEFAULT);
            Sort.currentCalcDelegate = Sort.GenerateCalcDelegate(customSortOrderingEntry.Value);
            customSortOrderingEntry.OnEntryValueChanged.Subscribe((_, newOrdering) =>
            {
                Sort.currentCalcDelegate = Sort.GenerateCalcDelegate(newOrdering);
            });
        }

        public override string Name => "MQOD - Custom Sort Settings";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;


        protected override void LateConstructUI()
        {
            createHotkey("Sorting", sortingKeyEntry);
            createHotkey("toggleAutoSorting [enabled]", toggleAutoSortingKeyEntry, out toggleAutoSortingLabel);

            List<Text> texts = new();
            Dictionary<Text, Sort.Category> CategoryIndex = new();

            foreach (Sort.Category category in customSortOrderingEntry.Value)
            {
                Text text = UIFactory.CreateLabel(ContentRoot, category.GetString(), category.GetString(),
                    fontSize: fontSize);
                ComponentDragController daComponentDragController =
                    text.gameObject.AddComponent<ComponentDragController>();
                daComponentDragController.currentTransform = text.rectTransform;
                UIFactory.SetLayoutElement(text.gameObject, 25, 25, 1);

                CategoryIndex[text] = category;
                texts.Add(text);

                daComponentDragController.callback = () =>
                {
                    Sort.Ordering oldOrdering = customSortOrderingEntry.Value;
                    Sort.Ordering newOrdering = new(oldOrdering.Count);
                    foreach (Text textIter in texts)
                    {
                        int oldIdx = oldOrdering.IndexOf(CategoryIndex[textIter]);
                        int newIdx = textIter.transform.GetSiblingIndex() - 3; // 1 offset + 2 components (hotkeys)
                        // MelonLogger.Msg($"{oldIdx} {newIdx} {oldOrdering.Count} {newOrdering.Count}");
                        newOrdering[newIdx] = oldOrdering[oldIdx];
                    }

                    customSortOrderingEntry.Value = newOrdering;
                };
            }

            base.LateConstructUI();
        }
    }
}