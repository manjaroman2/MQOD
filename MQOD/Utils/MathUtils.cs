using System.Linq;

namespace MQOD
{
    public static class MathUtils
    {
        public class ValueSmoother
        {
            private readonly int smoothing;
            private readonly float[] values;
            private int idx;
            private float value;

            public ValueSmoother(int smoothing)
            {
                this.smoothing = smoothing;
                values = new float[smoothing];
            }

            public float add(float Value)
            {
                values[idx] = Value;
                idx++;
                if (idx >= smoothing) idx = 0;
                value = values.Sum() / smoothing;
                return value;
            }
        }
    }
}