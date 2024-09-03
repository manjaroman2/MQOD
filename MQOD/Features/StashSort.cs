using Death.App;
using Death.Items;
using Death.Run.UserInterface.Items;
using HarmonyLib;
using MelonLoader;

namespace MQOD
{
    public class StashSort : Feature, Hookable
    {
        private ItemController_Stash StashItemController;

        public void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Items_Stash), nameof(GUI_Items_Stash.Init), types: new[] { typeof(Profile) },
                postfixClazz: typeof(StashSort), postfixMethod: nameof(GUI_Items_Stash__Init__Postfix));
            HarmonyHelper.Patch(typeof(ItemController_Stash), "Transfer", types: new[] { typeof(ItemSlot) },
                postfixClazz: typeof(StashSort), postfixMethod: nameof(ItemController_Stash__Transfer__Postfix));
        }


        public void sortSelectedPage()
        {
            if (StashItemController == null)
            {
                MelonLogger.Error("StashItemController is null, that's terrible!");
                return;
            }

            if (Sort.sortItemGrid(StashItemController.SelectedPage))
            {
                MelonLogger.Msg("Sorting Page=" + StashItemController.SelectedPage);
            }
            else
            {
                MelonLogger.Msg("Nothing to sort :)");
            }
        }
        /*
        public static void addDropdown(GUI_StashTabManager stashTabManager, GameObject GObj_GUI_Panel_Stash)
        {
            GameObject tabFab = (GameObject)typeof(GUI_StashTabManager).BaseType?.GetField("_tabFab", AccessTools.all)
                ?.GetValue(stashTabManager);
            GameObject tabFab__Text = tabFab?.transform.GetChild(3).gameObject;
            TextMeshProUGUI GUI_StashTab_Text_TMProUGUI = tabFab__Text?.GetComponent<TextMeshProUGUI>();
            GameObject GObj_GUI_Dropdown = new()
            {
                name = "GObj_GUI_Dropdown",
                layer = GObj_GUI_Panel_Stash.transform.parent.gameObject.layer
            };
            GObj_GUI_Dropdown.transform.SetParent(GObj_GUI_Panel_Stash.transform);
            GObj_GUI_Dropdown.transform.localScale = new Vector3(1, 1, 1);
            GObj_GUI_Dropdown.SetActive(false);
            GUI_DropDown guiDropDown = GObj_GUI_Dropdown.AddComponent<GUI_DropDown>();
            // GUI_DropDown.GenerateEntries(new Dictionary<string, Func<bool, bool>>{ {  "a", b => b } })
            guiDropDown.Init(new GUI_DropDown.Entry[]
            {
                new("Uniqueness"),
                new("Rarity"),
                new("Tier"),
                new("Type"),
                new("Subtype")
            }, GUI_StashTab_Text_TMProUGUI);
            GObj_GUI_Dropdown.SetActive(true);
            guiDropDown.enabled = true;
            MelonLogger.Msg("added Dropdown");
        }

         *
         */

        private static void GUI_Items_Stash__Init__Postfix(Profile profile,
            ref ItemController_Stash ____controller)
        {
            MQOD.Instance.StashSortInst.StashItemController = ____controller;
            // addDropdown(____stashTabManager, GameObject.Find("GUI_Panel_Stash"));
        }
        private static void ItemController_Stash__Transfer__Postfix(ItemSlot slot,
            ItemController_Stash __instance)
        {
            if (MQOD.Instance.SortedItemGridInst.isEnabled())
            {
                MQOD.Instance.StashSortInst.sortSelectedPage();
            }
        }
    }
}