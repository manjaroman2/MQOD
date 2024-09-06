using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class GemRadiusCreator : MonoBehaviour
    {
        public float Scale
        {
            set
            {
                _Scale = value;
                setQuadScale();
                updateWidth();
            }
        }

        public void setParentObject(GameObject obj)
        {
            parentObject = obj;
        }

        private float _Width = 0.05f;
        private float _Scale;
        private Shader GemRadiusShader;
        private GameObject parentObject;
        private GameObject quad;
        private Material material;

        public void updateWidth()
        {
            _Width = 1 / _Scale;
            if (MQOD.Instance.UI.FeatureGemVisualizer != null)
            {
                _Width *= 2 * MQOD.Instance.UI.FeatureGemVisualizer.widthModifier;
            }

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

        private void Start()
        {
            MelonLogger.Msg("GemRadiusCreator: Init");

            if (GemRadiusShader == null)
            {
                GemRadiusShader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("SimpleCircleShader");
                MelonLogger.Msg($"GemRadiusCreator: Init {GemRadiusShader.name}");
            }


            if (quad == null)
            {
                MelonLogger.Msg("GemRadiusCreator: Init Quad");
                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                material = new Material(GemRadiusShader);
                updateWidth();
                material.SetColor("_Color", (Vector4)new Color(20, 0, 0, 1));
                quad.GetComponent<Renderer>().material = material;
                quad.GetComponent<Renderer>().sortingLayerName = "Units";
                quad.transform.SetParent(parentObject.transform);
                quad.transform.localPosition = Vector3.zero;
                quad.transform.rotation = Quaternion.identity;
                setQuadScale();
            }

            MelonLogger.Msg("GemRadiusCreator: Initialized!");
        }
    }
}