using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntMathRandom : EntMath
    {
        private enum Mode { Int, Float }

        protected override Texture Icon => GetIcon("IconRandom");

        [PropertySpace(8)]

        [SerializeField] private Mode mode = default;

        [SerializeField] [ShowIf(nameof(IntMode)), LabelText("Min")] private int intMin;
        [SerializeField] [ShowIf(nameof(IntMode)), LabelText("Max")] private int intMax;

        [SerializeField] [ShowIf(nameof(FloatMode)), LabelText("Min")] private float floatMin;
        [SerializeField] [ShowIf(nameof(FloatMode)), LabelText("Max")] private float floatMax;

        protected override object Value
        {
            get
            {
                switch (mode)
                {
                    default: return null;
                    case Mode.Int: return intValue;
                    case Mode.Float: return floatValue;
                }
            }
            set
            {
                switch (mode)
                {
                    default: return;
                    case Mode.Int: intValue = (int)value; return;
                    case Mode.Float: floatValue = (float)value; return;
                }
            }
        }

        public bool IntMode() { return mode == Mode.Int; }
        public bool FloatMode() { return mode == Mode.Float; }

        [Route]
        public void Random()
        {
            switch (mode)
            {
                default: return;
                case Mode.Int:
                    LaunchValue(Jape.Random.Int(intMin, intMax));
                    break;

                case Mode.Float:
                    LaunchValue(Jape.Random.Float(floatMin, floatMax));
                    break;
            }
        }

        [Route]
        public void RandomInt(int min, int max) { LaunchValue(Jape.Random.Int(min, max)); }

        [Route]
        public void RandomFloat(float min, float max) { LaunchValue(Jape.Random.Float(min, max)); }

        [Route]
        public void SetMin(object input)
        {
            switch (mode)
            {
                default: return;
                case Mode.Int:
                    intMin = (int)input;
                    break;

                case Mode.Float:
                    floatMin = (float)input;
                    break;
            }
        }

        [Route]
        public void SetMax(object input)
        {
            switch (mode)
            {
                default: return;
                case Mode.Int:
                    intMax = (int)input;
                    break;

                case Mode.Float:
                    floatMax = (float)input;
                    break;
            }
        }
    }
}