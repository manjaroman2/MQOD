using Death.Run.Behaviours;

namespace MQOD
{
    public class CameraZoom : _Feature
    {
        private const int maxZoomState = 4;
        private float defaultZoom;
        private int zoomState = 1;
        private const float zoomScalar = 0.8f; 

        protected override void addHarmonyHooks()
        {
            HarmonyHelper.Patch(typeof(RunCamera), "Start", postfixClazz: typeof(CameraZoom),
                postfixMethod: nameof(RunCamera__Start__Postfix));
        }

        protected void init()
        {
            defaultZoom = RunCamera.Instance.OrthographicSize;
            initialized = true;
        }

        public void zoomOut()
        {
            if (!initialized) return;
            if (zoomState <= maxZoomState)
            {
                RunCamera.Instance.OrthographicSize = defaultZoom + zoomState * zoomScalar;
                zoomState++;
            }
            else
            {
                RunCamera.Instance.OrthographicSize = defaultZoom;
                zoomState = 1;
            }
        }

        private static void RunCamera__Start__Postfix()
        {
            if (MQOD.Instance.CameraZoomInst != null) MQOD.Instance.CameraZoomInst.init();
        }
    }
}