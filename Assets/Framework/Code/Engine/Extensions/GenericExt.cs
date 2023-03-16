using System;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public static class GenericExt
    {
        public static T CloneConvert<T>(this T original) where T : class, ICloneable { return original.Clone() as T; }

        public static T[,] ToMatrixColumns<T>(this IEnumerable<T> enumerable, int columns)
        {
            T[] array = enumerable.ToArray();
            int rows = array.Length / columns;
            rows += array.Length.IsDivisible(columns) ? 0 : 1;
            T[,] output = new T[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (i * columns) + j;
                    if (x >= array.Length) { return output; }
                    output[j, i] = array[x];
                }
            }
            return output;
        }
    }
}


