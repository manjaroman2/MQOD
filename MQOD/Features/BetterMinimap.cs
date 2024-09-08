using System.Reflection;
using Death.ResourceManagement;
using Death.Run.UserInterface.HUD.Minimap;
using Death.UserInterface;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MQOD
{
    public class BetterMinimap : _Feature
    {
        public Image boundsImage;
        public Color boundsImage_color;
        private RectTransform boundsImageRectTransform;
        private Vector2 boundsImageRectTransform_pivot;
        private Vector2 boundsImageRectTransform_sizeDelta;
        private FieldInfo boundsWidthAccessor;
        private float boundsWidthAccessor_GetValue;
        private GUI_Minimap.Config config;
        private float config_MapDimensionUnits;
        private GUI_Minimap guiMinimap;
        private GameObject Img_Frame;
        private Outline outline;
        private RectTransform rectTransform;
        private Vector2 rectTransform_anchorMax;

        private Vector2 rectTransform_anchorMin;
        private Vector2 rectTransform_pivot;
        public bool zoomedIn;

        public void init()
        {
            guiMinimap = Minimap.Get() as GUI_Minimap;
            if (guiMinimap == null)
            {
                MelonLogger.Error("BetterMinimap: GUI_Minimap null");
                return;
            }

            Img_Frame = guiMinimap.gameObject.transform.Find("Img_Frame").gameObject;

            if (Img_Frame == null)
            {
                MelonLogger.Error("BetterMinimap: Img_Frame null");
                return;
            }

            boundsImage =
                typeof(GUI_Minimap).GetField("_boundsImage", AccessTools.all)?.GetValue(guiMinimap) as Image;
            if (boundsImage == null)
            {
                MelonLogger.Error("BetterMinimap: boundsImage null");
                return;
            }

            boundsWidthAccessor = typeof(GUI_Minimap).GetField("_boundsWidth", AccessTools.all);
            if (boundsWidthAccessor == null)
            {
                MelonLogger.Error("BetterMinimap: boundsWidthAccessor null");
                return;
            }

            initialized = true;
        }

        public void zoomOut()
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            config ??= ConfigManager.Get<UIConfig>().Minimap;
            config.MapDimensionUnits += 10;
        }

        public void zoomIn()
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            config ??= ConfigManager.Get<UIConfig>().Minimap;
            config.MapDimensionUnits -= 10;
        }

        public void fullscreenMinimap(float scalar = 4f)
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            if (zoomedIn)
                // MelonLogger.Error("BetterMinimap: already zoomed in");
                return;

            Img_Frame.SetActive(false);

            if (rectTransform == null) rectTransform = guiMinimap.GetComponent<RectTransform>();
            rectTransform_anchorMin = rectTransform.anchorMin;
            rectTransform_anchorMax = rectTransform.anchorMax;
            rectTransform_pivot = rectTransform.pivot;
            rectTransform.anchorMin = new Vector2(0.50f, 0.50f);
            rectTransform.anchorMax = new Vector2(0.50f, 0.50f);
            rectTransform.pivot = new Vector2(0.50f, 0.50f);
            if (boundsImageRectTransform == null)
                boundsImageRectTransform = boundsImage.gameObject.GetComponent<RectTransform>();
            boundsImageRectTransform_sizeDelta = boundsImageRectTransform.sizeDelta;
            boundsImageRectTransform_pivot = boundsImageRectTransform.pivot;
            boundsImageRectTransform.pivot = new Vector2(0.50f, 0.50f);
            boundsImageRectTransform.sizeDelta = new Vector2(801, 801);
            boundsWidthAccessor_GetValue = (float)boundsWidthAccessor.GetValue(guiMinimap);
            boundsWidthAccessor.SetValue(guiMinimap, boundsWidthAccessor_GetValue * scalar);
            config ??= ConfigManager.Get<UIConfig>().Minimap;
            config_MapDimensionUnits = config.MapDimensionUnits;
            config.MapDimensionUnits *= scalar;
            boundsImage_color = boundsImage.color;
            boundsImage.color = new Color(boundsImage_color.r, boundsImage_color.g, boundsImage_color.b,
                MQOD.Instance.UIInst.FeatureMinimap.minimapTransparencyEntry.Value);
            // if (outline == null)
            // {
            //     outline = boundsImage.gameObject.AddComponent<Outline>();
            //     outline.effectColor = Color.red;
            //     outline.effectDistance = new Vector2(1, -1);
            //     outline.useGraphicAlpha = true;
            // }
            // outline.enabled = true;

            zoomedIn = true;
        }

        public void resetFullscreen()
        {
            if (!initialized || !zoomedIn) return;
            Img_Frame.SetActive(true);
            rectTransform.anchorMin = rectTransform_anchorMin;
            rectTransform.anchorMax = rectTransform_anchorMax;
            rectTransform.pivot = rectTransform_pivot;
            boundsImageRectTransform.pivot = boundsImageRectTransform_pivot;
            boundsImageRectTransform.sizeDelta = boundsImageRectTransform_sizeDelta;
            boundsWidthAccessor.SetValue(guiMinimap, boundsWidthAccessor_GetValue);
            config.MapDimensionUnits = config_MapDimensionUnits;
            boundsImage.color = boundsImage_color;
            // outline.enabled = false;

            zoomedIn = false;
        }

        // private static void MapObject_Doodad__Init__Postfix(MapObjectData data, bool isExhausted,
        //     MapObject_Doodad __instance)
        // {
        //     MelonLogger.Msg(__instance.transform);
        // }
        protected override void addHarmonyHooks()
        {
        }
    }
}