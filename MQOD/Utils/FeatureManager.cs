using System.Collections.Generic;
using MelonLoader;

namespace MQOD
{
    public class FeatureManager
    {
        private readonly List<_Feature> Features = new();

        public T addFeature<T>() where T : _Feature, new()
        {
            T tea = new();
            Features.Add(tea);
            return tea;
        }

        public void addHarmonyHooks()
        {
            foreach (_Feature feature in Features)
            {
                MelonLogger.Msg($"Adding Harmony hooks for {feature}");
                feature.applyHarmonyHooks();
            }
        }
    }
}