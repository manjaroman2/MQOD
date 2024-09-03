using System.Reflection;
using Death.Run.Behaviours;
using Death.Run.Behaviours.Entities;
using HarmonyLib;

namespace MQOD
{
    public class GemRadiusVisualizer : _Feature, _Hookable
    {
        private static readonly FieldInfo BGC_Radius_Accessor =
            typeof(Behaviour_GemCollector).GetField("Radius", AccessTools.all);

        public void addHarmonyHooks()
        {
        }


        public void init()
        {
            Behaviour_GemCollector behaviourGemCollector =
                Player.Instance.XpTracker.GetComponent<Behaviour_GemCollector>();
            float Radius = (float)BGC_Radius_Accessor.GetValue(behaviourGemCollector);

            initialized = true;
        }
    }
}