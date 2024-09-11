using System.IO;
using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class AssetManager
    {
        public AssetBundle bundle;

        public void init()
        {
            MelonLogger.Msg("AssetManager Init");
            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Application.streamingAssetsPath,
                Path.Combine(Application.dataPath, "../Mods/MQOD")), "MQODAssets"));
            if (bundle == null)
                MelonLogger.Error("Could not load MQOD asset bundle");
            else
                foreach (string allAssetName in bundle.GetAllAssetNames())
                    MelonLogger.Msg(allAssetName + " " + bundle.LoadAsset(allAssetName).GetType());
        }
    }
}