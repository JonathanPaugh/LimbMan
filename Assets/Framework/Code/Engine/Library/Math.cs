using UnityEngine;

namespace Jape
{
    public static class Math
    {
        public static bool IsEven(this int value) { return value % 2 == 0; }
        public static bool IsEven(this long value) { return value % 2 == 0; }

        public static bool IsDivisible(this int value, int divisor) { return value % divisor == 0; }
        public static bool IsDivisible(this long value, int divisor) { return value % divisor == 0; }

        public static bool IsPowerOfTwo(this int value) { return (value & (value - 1)) == 0; }
        public static bool IsPowerOfTwo(this long value) { return (value & (value - 1)) == 0; }

        public static bool IsSingleBit(this int value, bool includeZero = false)
        {
            switch (value)
            {
                case 0: return includeZero;
                case 1: return true;
                default: return value.IsPowerOfTwo();
            }
        }

        public static bool IsSingleBit(this long value, bool includeZero = false)
        {
            switch (value)
            {
                case 0: return includeZero;
                case 1: return true;
                default: return value.IsPowerOfTwo();
            }
        }

        public static bool SameSign(int value1, int value2) { return value1 * value2 >= 0; }
        public static bool SameSign(long value1, long value2) { return value1 * value2 >= 0; }
        public static bool SameSign(float value1, float value2) { return value1 * value2 >= 0f; }

        public static bool HasBit(this int value, int bit) { return (value & bit) != 0; }
        public static bool HasBit(this long value, long bit) { return (value & bit) != 0; }

        public static int RoundInterval(this int value, int interval) { return (int)Mathf.Round((float)value / interval) * interval; }
        public static int RoundSpecial(this int value) { return value.IsDivisible(5) ? value : value.RoundInterval(2); }

        public static float Abs(this float value) { return Mathf.Abs(value); }
        public static float Opp(this float value) { return value * -1; }

        public static float Parabola(float a, float b, float c, float x) { return (a * Mathf.Pow(x, 2)) + (b * x) + c; }

        public static float Rescale(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return ((value - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;
        }
        
        public static float Mimic(float value, float target)
        {
            if (target > 0) { return value.Abs(); }
            if (target < 0) { return -value.Abs(); }

            return value;
        }

        public static float Angle(Vector2 origin, Vector2 target) { return 180 / Mathf.PI * Mathf.Atan2(target.y - origin.y, target.x - origin.x); }
    }
}