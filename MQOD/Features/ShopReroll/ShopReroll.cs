using Death.Run.UserInterface.Items;
using Death.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace MQOD
{
    public class ShopReroll : _Feature
    {
        private static ShopRerollButton shopRerollButton;


        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(GUI_Shop), nameof(GUI_Shop.Init),
                new[] { typeof(ShopData), typeof(ItemController_Shop) }, typeof(ShopReroll),
                nameof(Postfix__GUI_Shop__Init));
        }

        private static void Postfix__GUI_Shop__Init(ShopData shop, ItemController_Shop controller, GUI_Shop __instance)
        {
            GameObject gameObject = new()
            {
                name = "ShopRerollButton"
            };
            gameObject.transform.SetParent(__instance.gameObject.transform);
            shopRerollButton = gameObject.AddComponent<ShopRerollButton>();
            shopRerollButton.enabled = true;
        }

        public class ShopRerollButton : MonoBehaviour
        {
            private void Start()
            {
                Button button = gameObject.AddComponent<Button>();
                Sprite sprite = MQOD.Instance.assetManager.bundle.LoadAsset<Sprite>("Shop_Reroll_Sprite");
            }
        }
    }
}