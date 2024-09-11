using Death.App;
using Death.Items;
using Death.Run.UserInterface.Items;
using MelonLoader;

namespace MQOD
{
    public class SortStash : _Feature
    {
        private ItemController_Stash StashItemController;

        public void sortSelectedPage()
        {
            if (StashItemController == null)
            {
                MelonLogger.Error("StashItemController is null, that's terrible!");
                return;
            }

            if (!Sort.sortItemGrid(StashItemController.SelectedPage))
                MelonLogger.Msg("Nothing to sort :)");
        }

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Stash), nameof(GUI_Items_Stash.Init), new[] { typeof(Profile) },
                postfixClazz: typeof(SortStash), postfixMethod: nameof(GUI_Items_Stash__Init__Postfix));
            HarmonyHelper.Patch(typeof(ItemController_Stash), "Transfer", new[] { typeof(ItemSlot) },
                postfixClazz: typeof(SortStash), postfixMethod: nameof(ItemController_Stash__Transfer__Postfix));
        }

        private static void GUI_Items_Stash__Init__Postfix(Profile profile,
            ref ItemController_Stash ____controller)
        {
            MQOD.Instance.SortStashInst.StashItemController = ____controller;
        }

        private static void ItemController_Stash__Transfer__Postfix(ItemSlot slot,
            ItemController_Stash __instance)
        {
            if (MQOD.Instance.SortItemGridInst.isEnabled()) MQOD.Instance.SortStashInst.sortSelectedPage();
        }
    }
}