using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
	public static class EnumExt
    {
        public static int ConvertFromFlags(this Enum @enum)
        {
            int value = Convert.ToInt32(@enum);
            switch (value)
            {
                case 0: return value;
                default: return Convert.ToInt32(Mathf.Log10(value * 2) / Mathf.Log10(2));
            }
        }

        public static int ConvertToFlags(this Enum @enum)
        {
            int value = Convert.ToInt32(@enum);
            switch (value)
            {
                case 0: return value;
                default: return (int)System.Math.Pow(2, value - 1);
            }
        }

        public static IEnumerable<string> FlagValues(this Enum @enum, bool includeNone = false)
        {
            IEnumerable<string> values = Enum.GetValues(@enum.GetType()).Cast<Enum>().Where(@enum.HasFlag).Select(e => e.ToString());
            return includeNone ? values : values.Where(n => n != "None");
        }
    }
}