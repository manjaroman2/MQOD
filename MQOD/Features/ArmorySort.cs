using Death.App;
using Death.Run.UserInterface.Items;
using MelonLoader;

namespace MQOD
{
    public class ArmorySort : _Feature, _Hookable
    {
        public ItemController_Armory ItemControllerArmory;

        public void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Armory), nameof(GUI_Items_Armory.Init), new[] { typeof(Profile) },
                postfixClazz: typeof(ArmorySort), postfixMethod: nameof(GUI_Items_Armory__Init__Postfix));
        }

        public void sort()
        {
            MelonLogger.Msg(MQOD.Instance.mqodUI.SortPanel.SortOrdering.sortItemGrid(ItemControllerArmory.ArmoryGrid)
                ? "Sorted Armory"
                : "There is nothing to sort :)");
        }


        private static void GUI_Items_Armory__Init__Postfix(Profile profile,
            ref ItemController_Armory ____controller)
        {
            MQOD.Instance.ArmorySortInst.ItemControllerArmory = ____controller;
        }
    }
}