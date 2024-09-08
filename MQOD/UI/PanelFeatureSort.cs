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

            Sort.Ordering SortOrdering = customSortOrderingEntry.Value;
            MelonLogger.Msg(SortOrdering);
            foreach (Sort.Category category in SortOrdering)
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

            base.LateConstructUI();
        }
    }
}