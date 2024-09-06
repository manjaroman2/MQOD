using UnityEngine;
using UniverseLib.UI;

namespace MQOD
{
    public class PanelFeatureCamera : PanelBaseMQOD
    {
        public PanelFeatureCamera(UIBase owner) : base(owner)
        {
        }

        public override string Name => "MQOD - Camera Settings";
        public override int MinWidth => 300;
        public override int MinHeight => 300;
        public override Vector2 DefaultAnchorMin => new(0.10f, 0.90f);
        public override Vector2 DefaultAnchorMax => new(0.10f, 0.90f);
        public override bool CanDragAndResize => true;

        protected override void ConstructPanelContent()
        {
            base.ConstructPanelContent();
        }
    }
}