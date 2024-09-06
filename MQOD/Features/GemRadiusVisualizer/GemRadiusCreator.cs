using MelonLoader;
using UnityEngine;

namespace MQOD
{
    public class GemRadiusCreator : MonoBehaviour
    {
        public static readonly int __Width = Shader.PropertyToID("_Width");
        private static readonly int __ColorOuter = Shader.PropertyToID("_ColorOuter");

        private static readonly int __ColorInner = Shader.PropertyToID("_ColorInner");
        public GameObject quad;

        private float _Scale;

        // public Material EllipticShaderMaterial;
        private Shader GemRadiusShader;
        private GameObject parentObject;

        public float Scale
        {
            set
            {
                _Scale = value;
                setQuadScale();
            }
        }


        private void Start()
        {
            // Shader shader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("TestShader");
            // material = new Material(shader);
            // Shader shader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("CircleShader");
            // Shader shader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("EllipticShader");
            // Material material = new(shader);
            // material.EnableKeyword("_ALPHATEST_ON");
            // material.SetFloat(AlphaClip, 1);

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
                Material material = new(GemRadiusShader);
                material.SetFloat("_Width", 0.05f);
                material.SetColor("_Color", (Vector4)new Color(1.0f, 0.7490196f, 0.04705882f, 1.0f));
                quad.GetComponent<Renderer>().material = material;
                quad.GetComponent<Renderer>().sortingLayerName = "Units";
                quad.transform.SetParent(parentObject.transform);
                quad.transform.localPosition = Vector3.zero;
                quad.transform.rotation = Quaternion.identity;
                setQuadScale();
            }


            // if (EllipticShaderMaterial == null)
            // {
            //     MelonLogger.Msg("GemRadiusCreator: Init EllipticShaderMaterial"); 
            //     EllipticShaderMaterial =
            //         MQOD.Instance.assetManager.bundle.LoadAsset<Material>("EllipticShaderMaterial");
            // }

            // if (quad == null)
            // {
            //     MelonLogger.Msg("GemRadiusCreator: Init Quad");
            //     quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //     quad.GetComponent<Renderer>().material = EllipticShaderMaterial;
            //     quad.GetComponent<Renderer>().sortingLayerName = "Units";
            //     quad.transform.SetParent(parentObject.transform);
            //     quad.transform.localPosition = Vector3.zero;
            //     quad.transform.rotation = Quaternion.identity;
            // }

            MelonLogger.Msg("GemRadiusCreator: Initialized!");
        }

        // public void setShaderWidth(float Width)
        // {
        //     if (EllipticShaderMaterial != null)
        //     {
        //         EllipticShaderMaterial.SetFloat(__Width, Width);
        //     }
        // }
        private void setQuadScale()
        {
            if (quad != null) quad.transform.localScale = new Vector3(_Scale, _Scale, _Scale);
        }

        public void setParentObject(GameObject obj)
        {
            parentObject = obj;
        }
    }
}