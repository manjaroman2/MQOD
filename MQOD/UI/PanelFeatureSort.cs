using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureSort : PanelBaseMQOD
    {
        public Text toggleAutoSortingLabel;

        public PanelFeatureSort(UIBase owner) : base(owner)
        {
        }

        public Sort.Ordering SortOrdering
        {
            get => MQOD.Instance.preferencesManager.customSortOrderingEntry.Value;
            set => MQOD.Instance.preferencesManager.customSortOrderingEntry.Value = value;
        }

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

        public override string Name => "MQOD - Custom Sort Settings";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();
            createHotkey("Sorting", () => sortingKey, code => sortingKey = code);
            createHotkey("toggleAutoSorting [enabled]", () => toggleAutoSortingKey, code => toggleAutoSortingKey = code,
                out toggleAutoSortingLabel);

            List<Text> texts = new();
            Dictionary<Text, Sort.Category> CategoryIndex = new();

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
        }

        public void loadVariables(Sort.Ordering entries)
        {
            SortOrdering = entries;
        }
    }
}