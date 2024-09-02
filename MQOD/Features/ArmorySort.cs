using System.Collections.Generic;
using Death.App;
using Death.Items;
using Death.Run.UserInterface.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public class ArmorySort : Feature, Hookable
    {
        public ItemController_Armory ItemControllerArmory;

        public void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Armory), nameof(GUI_Items_Armory.Init), new[] { typeof(Profile) },
                postfixClazz: typeof(ArmorySort), postfixMethod: nameof(GUI_Items_Armory__Init__Postfix));
        }

        public void sort()
        {
            MelonLogger.Msg(Sort.sortItemGrid(ItemControllerArmory.ArmoryGrid)
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