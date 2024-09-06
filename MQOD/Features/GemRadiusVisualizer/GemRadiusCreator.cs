using UnityEngine;

namespace MQOD
{
    public class GemRadiusCreator : MonoBehaviour
    {
        public GameObject quad;
        private float _Scale;

        private float _Width = 0.05f;
        private Shader GemRadiusShader;
        private Material material;
        private GameObject parentObject;

        public float Scale
        {
            set
            {
                _Scale = value;
                setQuadScale();
                updateWidth();
            }
        }

        private void Start()
        {
            // MelonLogger.Msg("GemRadiusCreator: Init");

            if (GemRadiusShader == null)
                GemRadiusShader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("SimpleCircleShader");
            // MelonLogger.Msg($"GemRadiusCreator: Init {GemRadiusShader.name}");

            if (quad == null)
            {
                // MelonLogger.Msg("GemRadiusCreator: Init Quad");
                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                material = new Material(GemRadiusShader);
                updateWidth();

                MQOD.Instance.preferencesManager.gemRadiusColorFloat.OnEntryValueChanged.Subscribe((_, newVal) =>
                {
                    material.SetColor("_Color", PanelBaseMQOD.FlatToColor(newVal));
                });
                material.SetColor("_Color",
                    PanelBaseMQOD.FlatToColor(MQOD.Instance.preferencesManager.gemRadiusColorFloat.Value));
                quad.GetComponent<Renderer>().material = material;
                quad.GetComponent<Renderer>().sortingLayerName = "Units";
                quad.transform.SetParent(parentObject.transform);
                quad.transform.localPosition = Vector3.zero;
                quad.transform.rotation = Quaternion.identity;
                quad.SetActive(MQOD.Instance.GemRadiusVisualizerInst.Shown.Value);
                setQuadScale();
            }

            // MelonLogger.Msg("GemRadiusCreator: Initialized!");
        }

        public void setParentObject(GameObject obj)
        {
            parentObject = obj;
        }

        public void updateWidth()
        {
            _Width = 1 / _Scale;
            if (MQOD.Instance.UI.FeatureGemVisualizer != null) _Width *= 2 * PanelFeatureGemVisualizer.widthModifier;

            setWidth();
        }

        private void setWidth()
        {
            if (material != null) material.SetFloat("_Width", _Width);
        }


        private void setQuadScale()
        {
            if (quad != null) quad.transform.localScale = new Vector3(_Scale, _Scale, _Scale);
        }
    }
}