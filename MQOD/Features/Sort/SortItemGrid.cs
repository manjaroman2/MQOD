using System.Collections.Generic;
using Death.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public class SortItemGrid : _Feature
    {
        private bool enabled = true;

        protected override void addHarmonyHooks()
        {
            if (enabled) enableSorting();
        }

        public bool isEnabled()
        {
            return enabled;
        }

        public void toggleSorting()
        {
            enabled = !enabled;
            applySorting();
        }

        private void applySorting()
        {
            if (enabled) enableSorting();
            else disableSorting();
        }

        private static void enableSorting()
        {
            HarmonyHelper.Patch(typeof(ItemGrid), nameof(ItemGrid.Populate), new[] { typeof(IEnumerable<Item>) },
                postfixClazz: typeof(SortItemGrid), postfixMethod: nameof(ItemGrid__Populate__Postfix));
        }

        private static void disableSorting()
        {
            MQOD.Instance.HarmonyInstance.Unpatch(typeof(ItemGrid).GetMethod(nameof(ItemGrid.Populate)),
                typeof(SortItemGrid).GetMethod(nameof(ItemGrid__Populate__Postfix), AccessTools.all));
        }

        private static void ItemGrid__Populate__Postfix(IEnumerable<Item> items, ref ItemGrid __instance)
        {
            MQOD.Instance.UI.FeatureSort.SortOrdering.sortItemGrid(__instance);
            // MelonLogger.Msg(MQOD.Instance.UI.FeatureSort.SortOrdering.sortItemGrid(__instance)
            //     ? "Sorted item grid"
            //     : "Nothing to sort :)");
        }
    }
}