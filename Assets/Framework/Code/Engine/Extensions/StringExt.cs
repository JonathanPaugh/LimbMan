using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Jape
{
	public static class StringExt
    {
        public static void Copy(this string value) { GUIUtility.systemCopyBuffer = value; }

        public static string RemoveNamespace(this string value) { return string.Join("", Regex.Split(value, @"([^\w\.])").Select(p => p.Substring(p.LastIndexOf('.') + 1))); }

        public static string RemoveSuffix(this string value) { return Regex.Replace(value, @"d__\d*", string.Empty); }

        public static string RemoveBrackets(this string value)
        {
            return value.Replace("<", string.Empty)
                        .Replace(">", string.Empty)
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .Replace("{", string.Empty)
                        .Replace("}", string.Empty)
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);
        }

        public static string RemoveSpaces(this string value) { return value.Replace(" ", string.Empty); }

        public static string SerializationName(this string value)
        {
            string temp = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                char letter = value[i];

                if (i == 0) { temp += letter.ToString().ToUpper(); }
                else { temp += letter; }
            }

            return temp;
        }
    }
}