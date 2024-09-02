using System.Collections.Generic;
using MelonLoader;

namespace MQOD
{
    public class FeatureManager
    {
        
        private readonly List<Feature> Features = new();
        
        public T addFeature<T>() where T : Feature, new() 
        {
            T tea = new();
            Features.Add(tea);
            return tea;
        }

        public void addHarmonyHooks()
        {
            foreach (Feature feature in Features)
                if (feature is Hookable hookableFeature)
                {
                    MelonLogger.Msg($"Adding Harmony hooks for {feature}");
                    hookableFeature.addHarmonyHooks();   
                }
        }

    }
}