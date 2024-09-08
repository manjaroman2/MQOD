using MelonLoader;
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
                updateWidth(MQOD.Instance.UIInst.FeatureGemVisualizer.widthModifierEntry.Value);
            }
        }

        private void Start()
        {
            // MelonLogger.Msg("GemRadiusCreator: Init");

            if (GemRadiusShader == null)
            {
                GemRadiusShader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>(
                    MQOD.Instance.GemRadiusVisualizerInst.ShaderOptions[
                        MQOD.Instance.GemRadiusVisualizerInst.ShaderNumber.Value]);
                applyShader();
                MQOD.Instance.GemRadiusVisualizerInst.ShaderNumber.OnEntryValueChanged.Subscribe((_, newVal) =>
                {
                    GemRadiusShader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>(
                        MQOD.Instance.GemRadiusVisualizerInst.ShaderOptions[newVal]);
                    applyShader();
                });
            }
            // MelonLogger.Msg($"GemRadiusCreator: Init {GemRadiusShader.name}");

            if (quad == null)
            {
                // MelonLogger.Msg("GemRadiusCreator: Init Quad");
                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
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

        private void applyShader()
        {
            MelonLogger.Msg("applyShader: " + GemRadiusShader);
            material = new Material(GemRadiusShader);
            updateWidth(MQOD.Instance.UIInst.FeatureGemVisualizer.widthModifierEntry.Value);

            material.SetColor("_Color",
                PanelBaseMQOD.FlatToColor(MQOD.Instance.UIInst.FeatureGemVisualizer.gemRadiusColorFloatEntry.Value));

            MQOD.Instance.UIInst.FeatureGemVisualizer.gemRadiusColorFloatEntry.OnEntryValueChanged.Subscribe(
                (_, newVal) => { material.SetColor("_Color", PanelBaseMQOD.FlatToColor(newVal)); });

            if (quad != null) quad.GetComponent<Renderer>().material = material;
        }

        public void setParentObject(GameObject obj)
        {
            parentObject = obj;
        }

        public void updateWidth(float widthModifier)
        {
            _Width = 1 / _Scale;
            _Width *= 2 * widthModifier;
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