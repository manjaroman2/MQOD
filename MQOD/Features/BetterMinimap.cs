using System;
using System.Reflection;
using System.Threading;
using Death.Data;
using Death.ResourceManagement;
using Death.Run.UserInterface.HUD.Minimap;
using Death.UserInterface;
using Death.Utils;
using Death.WorldGen;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MQOD
{
    public class BetterMinimap : _Feature
    {
        private const int MaxZoomState = 5;


        private static readonly FieldInfo chunkViewRangeAccessor =
            AccessTools.Field(typeof(WorldGenConfig), "_chunkViewRange");

        private static readonly FieldInfo chunkCleanupRangeAccessor =
            AccessTools.Field(typeof(WorldGenConfig), "_chunkCleanupRange");

        private readonly FieldInfo boundsImageAccessor = AccessTools.Field(typeof(GUI_Minimap), "_boundsImage");
        private readonly FieldInfo boundsWidthAccessor = AccessTools.Field(typeof(GUI_Minimap), "_boundsWidth");
        public Image boundsImage;
        public Color boundsImage_color;
        private RectTransform boundsImageRectTransform;
        private Vector2 boundsImageRectTransform_pivot;
        private Vector2 boundsImageRectTransform_sizeDelta;
        private float boundsWidthAccessor_GetValue;
        private GUI_Minimap.Config config;
        public CoordUtils Coords;
        private float default_config_MapDimensionUnits;
        private GUI_Minimap guiMinimap;
        private GameObject Img_Frame;
        public bool IsFullscreen;
        private float mapDimensionUnitsState;
        private RectTransform rectTransform;
        private Vector2 rectTransform_anchorMax;

        private Vector2 rectTransform_anchorMin;
        private Vector2 rectTransform_pivot;
        private int ZoomState;

        public Action<int> setChunkViewRange { get; private set; }
        public Func<int> getChunkViewRange { get; private set; }

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

            boundsImage = (Image)boundsImageAccessor.GetValue(guiMinimap);
            if (boundsImage == null)
            {
                MelonLogger.Error("BetterMinimap: boundsImage null");
                return;
            }

            config ??= ConfigManager.Get<UIConfig>().Minimap;
            default_config_MapDimensionUnits = config.MapDimensionUnits;
            initialized = true;
        }

        public void zoomOut()
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            if (ZoomState >= MaxZoomState) return;
            MelonLogger.Msg("ZoomOut");
            setChunkViewRange(getChunkViewRange() + 1);
            mapDimensionUnitsState += 30;
            if (IsFullscreen)
                config.MapDimensionUnits = Math.Max(mapDimensionUnitsState, default_config_MapDimensionUnits) * 4f;
            else config.MapDimensionUnits = Math.Max(mapDimensionUnitsState, default_config_MapDimensionUnits);
            MelonLogger.Msg("MapDimensionUnits: " + config.MapDimensionUnits);

            ZoomState++;
        }

        public void zoomIn()
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            if (ZoomState <= 0) return;
            MelonLogger.Msg("ZoomIn");
            setChunkViewRange(getChunkViewRange() - 1);
            mapDimensionUnitsState -= 30;
            if (IsFullscreen)
                config.MapDimensionUnits = Math.Max(mapDimensionUnitsState, default_config_MapDimensionUnits) * 4f;
            else config.MapDimensionUnits = Math.Max(mapDimensionUnitsState, default_config_MapDimensionUnits);
            MelonLogger.Msg("MapDimensionUnits: " + config.MapDimensionUnits);

            ZoomState--;
        }

        public void Fullscreen()
        {
            if (!initialized)
            {
                MelonLogger.Warning("BetterMinimap: not initialized");
                return;
            }

            if (IsFullscreen) return;

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

            boundsImageRectTransform.sizeDelta = new Vector2(Screen.height - 100, Screen.height - 100);
            boundsWidthAccessor_GetValue = (float)boundsWidthAccessor.GetValue(guiMinimap);
            boundsWidthAccessor.SetValue(guiMinimap, boundsWidthAccessor_GetValue * 4f);
            boundsImage_color = boundsImage.color;
            boundsImage.color = new Color(boundsImage_color.r, boundsImage_color.g, boundsImage_color.b,
                MQOD.Instance.UIInst.FeatureMinimap.minimapTransparencyEntry.Value);

            config.MapDimensionUnits = Math.Max(mapDimensionUnitsState, default_config_MapDimensionUnits) * 4f;
            MelonLogger.Msg("MapDimensionUnits: " + config.MapDimensionUnits);
            IsFullscreen = true;
        }

        public void unFullscreen()
        {
            if (!initialized || !IsFullscreen) return;
            Img_Frame.SetActive(true);
            rectTransform.anchorMin = rectTransform_anchorMin;
            rectTransform.anchorMax = rectTransform_anchorMax;
            rectTransform.pivot = rectTransform_pivot;
            boundsImageRectTransform.pivot = boundsImageRectTransform_pivot;
            boundsImageRectTransform.sizeDelta = boundsImageRectTransform_sizeDelta;
            boundsWidthAccessor.SetValue(guiMinimap, boundsWidthAccessor_GetValue);
            boundsImage.color = boundsImage_color;
            config.MapDimensionUnits = default_config_MapDimensionUnits;
            MelonLogger.Msg("MapDimensionUnits: " + config.MapDimensionUnits);
            IsFullscreen = false;
        }

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(WorldGenerator), nameof(WorldGenerator.InitAsync),
                new[] { typeof(WorldGenRecipe), typeof(Bounds2D) }, typeof(BetterMinimap),
                nameof(Postfix__WorldGenerator__InitAsync));
            HarmonyHelper.Patch(typeof(WorldGenerator), "RemoveChunk",
                new[] { typeof(Vector2Int) }, typeof(BetterMinimap),
                nameof(Postfix__WorldGenerator__RemoveChunk));
            HarmonyHelper.Patch(typeof(WorldGenerator), "GenerateChunkAsync",
                new[] { typeof(Vector2Int), typeof(CancellationToken) }, typeof(BetterMinimap),
                nameof(Postfix__WorldGenerator__GenerateChunkAsync));
            HarmonyHelper.Patch(typeof(WorldGenerator), "UpdateVisibleChunksAsync",
                new[] { typeof(Bounds2D), typeof(CancellationToken) }, typeof(BetterMinimap),
                nameof(Postfix__WorldGenerator__UpdateVisibleChunksAsync));
        }

        private static void Postfix__WorldGenerator__UpdateVisibleChunksAsync(
            Bounds2D cameraWorldBounds,
            CancellationToken token)
        {
            // MelonLogger.Msg($"============= Updating chunks =============");
            // MelonLogger.Msg($"cameraWorldBounds {cameraWorldBounds}");
            // MelonLogger.Msg(
            //     $"cameraWorldBounds {MQOD.Instance.BetterMinimapInst.Coords.WorldToChunk(cameraWorldBounds.Center)}");
        }

        private static void Postfix__WorldGenerator__RemoveChunk(Vector2Int coords)
        {
            // MelonLogger.Msg($"Removing chunk at {coords}");
        }

        private static void Postfix__WorldGenerator__GenerateChunkAsync(Vector2Int coords, CancellationToken token)
        {
            // MelonLogger.Msg($"Generating chunk at {coords}");
        }

        private static void Postfix__WorldGenerator__InitAsync(WorldGenerator __instance, WorldGenConfig ____config,
            WorldGenRecipe recipe, Bounds2D cameraWorldBounds)
        {
            Database.MapObjects.GetAllForAct(recipe.MapObjectAct);
            MQOD.Instance.BetterMinimapInst.Coords = ____config.Coords;
            MQOD.Instance.BetterMinimapInst.getChunkViewRange = () => (int)chunkViewRangeAccessor.GetValue(____config);
            MQOD.Instance.BetterMinimapInst.setChunkViewRange = c =>
            {
                MelonLogger.Msg("chunkViewRange: " + c);
                chunkViewRangeAccessor.SetValue(____config, c);
                chunkCleanupRangeAccessor.SetValue(____config, c + 1);
            };
        }
    }
}