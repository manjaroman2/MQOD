using Death.App;
using Death.Run.UserInterface.Items;
using MelonLoader;

namespace MQOD
{
    public class SortShop : _Feature, _Hookable
    {
        public ItemController_Shop ShopItemController;

        public void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Shop), nameof(GUI_Items_Shop.Init), new[] { typeof(Profile) },
                postfixClazz: typeof(SortShop), postfixMethod: nameof(GUI_Items_Shop__Init__Postfix));
            HarmonyHelper.Patch(typeof(Profile), nameof(Profile.ReGenerateShop), postfixClazz: typeof(SortShop),
                postfixMethod: nameof(Profile__ReGenerateShop__Postfix));
        }

        public static void sortShop()
        {
            if (MQOD.Instance.SortShopInst.ShopItemController == null)
            {
                MelonLogger.Error("MQOD.Instance.ShopSortInst.ShopItemController is null, how terrible!");
                return;
            }

            MelonLogger.Msg(
                MQOD.Instance.mqodUI.SortPanel.SortOrdering.sortItemGrid(MQOD.Instance.SortShopInst.ShopItemController
                    .GetSelectedPageGrid())
                    ? $"Sorted Shop {MQOD.Instance.SortShopInst.ShopItemController.SelectedPage}"
                    : "There was nothing to sort :)");
        }


        private static void Profile__ReGenerateShop__Postfix(ref Profile __instance)
        {
            MelonLogger.Msg("Profile__ReGenerateShop__Postfix");
            sortShop();
        }


        private static void GUI_Items_Shop__Init__Postfix(Profile profile,
            ref ItemController_Shop ____controller)
        {
            MQOD.Instance.SortShopInst.ShopItemController = ____controller;
        }
    }
}