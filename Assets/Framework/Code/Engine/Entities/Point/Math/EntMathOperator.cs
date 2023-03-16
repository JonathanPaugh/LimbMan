using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntMathOperator : EntMath
    {
        protected override Texture2D Icon => GetIcon("IconOperator");

        protected override object Value
        {
            get => floatValue;
            set => floatValue = (float)value;
        }

        [Route]
        public void Set(float input) { LaunchValue(input); }

        [Route]
        public void Add(float input) { LaunchValue(floatValue + input); }

        [Route]
        public void Subtract(float input) { LaunchValue(floatValue - input); }

        [Route]
        public void Multiply(float input) { LaunchValue(floatValue * input); }

        [Route]
        public void Divide(float input) { LaunchValue(floatValue /= input); }

        [Route]
        public void Absolute() { LaunchValue(floatValue.Abs()); }
    }
}