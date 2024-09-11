using Death.App;
using Death.Run.UserInterface.Items;
using MelonLoader;

namespace MQOD
{
    public class SortArmory : _Feature
    {
        public ItemController_Armory ItemControllerArmory;

        public void sort()
        {
            if (!Sort.sortItemGrid(ItemControllerArmory.ArmoryGrid)) MelonLogger.Msg("There is nothing to sort :)");
        }

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Armory), nameof(GUI_Items_Armory.Init), new[] { typeof(Profile) },
                postfixClazz: typeof(SortArmory), postfixMethod: nameof(GUI_Items_Armory__Init__Postfix));
        }


        private static void GUI_Items_Armory__Init__Postfix(Profile profile,
            ref ItemController_Armory ____controller)
        {
            MQOD.Instance.SortArmoryInst.ItemControllerArmory = ____controller;
        }
    }
}