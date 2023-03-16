using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntMathRescale : EntMath
    {
        [PropertySpace(8)]

        [SerializeField] private float oldMin;
        [SerializeField] private float oldMax;

        [PropertySpace(8)]

        [SerializeField] private float newMin;
        [SerializeField] private float newMax;

        protected override object Value
        {
            get => floatValue;
            set => floatValue = (float)value;
        }

        [Route]
        public void Rescale(float input) { LaunchValue(Math.Rescale(input, oldMin, oldMax, newMin, newMax)); }

        [Route]
        public void RescaleOnce(float input, float oldMin, float oldMax, float newMin, float newMax) { LaunchValue(Math.Rescale(input, oldMin, oldMax, newMin, newMax)); }

        [Route]
        public void SetOldMin(float input) { oldMin = input; }

        [Route]
        public void SetOldMax(float input) { oldMax = input; }

        [Route]
        public void SetNewMin(float input) { newMin = input; }

        [Route]
        public void SetNewMax(float input) { newMax = input; }
    }
}