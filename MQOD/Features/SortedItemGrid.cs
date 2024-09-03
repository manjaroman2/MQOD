using System.Collections.Generic;
using Death.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public class SortedItemGrid : _Feature, _Hookable
    {
        private bool enabled = true;

        public void addHarmonyHooks()
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
                postfixClazz: typeof(SortedItemGrid), postfixMethod: nameof(ItemGrid__Populate__Postfix));
        }

        private static void disableSorting()
        {
            MQOD.Instance.HarmonyInstance.Unpatch(typeof(ItemGrid).GetMethod(nameof(ItemGrid.Populate)),
                typeof(SortedItemGrid).GetMethod(nameof(ItemGrid__Populate__Postfix), AccessTools.all));
        }

        private static void ItemGrid__Populate__Postfix(IEnumerable<Item> items, ref ItemGrid __instance)
        {
            MelonLogger.Msg(MQOD.Instance.mqodUI.SortPanel.SortOrdering.sortItemGrid(__instance)
                ? "Sorted item grid"
                : "Nothing to sort :)");
        }
    }
}