using System;
using System.Timers;
using Death.App.UserInterface.Cursors;
using Death.Run.Behaviours;
using UnityEngine;

namespace MQOD
{
    public class MouseEffects : _Feature
    {
        private static readonly int RotationSpeed = Shader.PropertyToID("_RotationSpeed");
        private static readonly int FlareSpeed = Shader.PropertyToID("_FlareSpeed");
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");
        private static readonly int Dynamics = Shader.PropertyToID("_Dynamics");

        private readonly MathUtils.ValueSmoother RotationSmoother = new(5);
        private readonly Timer timer = new(1000) { AutoReset = false };
        public MouseEffectsComponent mouseEffectsComponent;

        public GameObject MouseEffectsGameObject;

        public void setState(bool active)
        {
            if (MouseEffectsGameObject != null) MouseEffectsGameObject.SetActive(active);
        }

        public void calcRotation(Vector2 delta)
        {
            if (delta.x != 0 || delta.y != 0)
            {
                float length = delta.magnitude;
                float theta = Mathf.Atan2(delta.y, delta.x);
                int sgn = Math.Sign(theta);
                float rotation = sgn * RotationSmoother.add(sgn * theta);
                mouseEffectsComponent.setRotation(rotation);
                mouseEffectsComponent.setDynamics(0);
                timer.Start();
            }

            if (!timer.Enabled) mouseEffectsComponent.setDynamics(1);
            mouseEffectsComponent.setLocalPos();
        }

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(CursorManager), "OnGameInit", postfixClazz: typeof(MouseEffects),
                postfixMethod: nameof(Postfix__CursorManager__OnGameInit));
        }

        private static void Postfix__CursorManager__OnGameInit(CursorManager __instance)
        {
            GameObject gameObject = new()
            {
                name = "MouseEffects"
            };
            gameObject.transform.SetParent(__instance.gameObject.transform);
            MQOD.Instance.MouseEffectsInst.mouseEffectsComponent = gameObject.AddComponent<MouseEffectsComponent>();
            MQOD.Instance.MouseEffectsInst.MouseEffectsGameObject = gameObject;
            MQOD.Instance.MouseEffectsInst.mouseEffectsComponent.enabled = true;
            MQOD.Instance.MouseEffectsInst.MouseEffectsGameObject.SetActive(MQOD.Instance.UIInst.FeatureMouseEffects
                .ToggleEntry.Value);
        }

        public class MouseEffectsComponent : MonoBehaviour
        {
            private const float PI_2 = Mathf.PI / 2;
            private Material material;
            private GameObject quad;

            private void Start()
            {
                Shader shader = MQOD.Instance.assetManager.bundle.LoadAsset<Shader>("FireRadial");
                material = new Material(shader);
                material.SetFloat(RotationSpeed, 1);
                material.SetFloat(FlareSpeed, 1);
                setRotation(0);
                setDynamics(1);
                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.GetComponent<Renderer>().material = material;
                quad.GetComponent<Renderer>().sortingLayerName = "OverlayEffects";
                quad.transform.rotation = Quaternion.identity;
                setLocalPos();
                quad.transform.SetParent(gameObject.transform);
                quad.SetActive(true);
                MQOD.Instance.MouseEffectsInst.initialized = true;
            }

            public void setDynamics(float dynamics)
            {
                material.SetFloat(Dynamics, dynamics);
            }

            public void setRotation(float rotation)
            {
                material.SetFloat(Rotation, rotation - PI_2);
            }

            public void setLocalPos()
            {
                if (RunCamera.Instance != null)
                    quad.transform.localPosition =
                        RunCamera.Instance.ScreenToWorldPoint(Input.mousePosition + new Vector3(5, -3, 0));
            }
        }
    }
}