using System;
using System.Globalization;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class Percent
    {
        [SerializeField, HideInInspector] protected float value;

        protected Percent (float value) { this.value = value; }

        public float Display() { return value * 100; }

        public static implicit operator Percent(float percentage) { return new Percent(percentage / 100); }
        public static implicit operator float(Percent percent) { return percent.value; }

        public override string ToString() { return value.ToString(CultureInfo.InvariantCulture); }
    }
}