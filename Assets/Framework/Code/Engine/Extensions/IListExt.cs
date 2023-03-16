using System.Collections.Generic;

namespace Jape
{
    public static class IListExt
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int number = Random.Int(0, i);
                (list[number], list[i]) = (list[i], list[number]);
            }
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.IndexOutOfRangeException("List is empty");
            }

            return list[Random.Int(0, list.Count - 1)];
        }

        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.IndexOutOfRangeException("List is empty");
            }

            int index = Random.Int(0, list.Count - 1);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }
}