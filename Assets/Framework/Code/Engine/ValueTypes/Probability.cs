using System;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class Probability : Percent
    {
        public static readonly Probability min = new(0);
        public static readonly Probability max = new(100);

        private Probability (float value) : base(value) { Mathf.Clamp(value, 0, 1); }

        public static implicit operator Probability(float percentage) { return new Probability(percentage / 100); }
        public static implicit operator float(Probability probability) { return probability.value; }
    }
}