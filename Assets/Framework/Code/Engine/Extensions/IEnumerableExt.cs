using System.Collections.Generic;
using System.Linq;

namespace Jape
{
	public static class IEnumerableExt
    {
        public static bool None<T>(this IEnumerable<T> source, System.Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static IEnumerable<T> Order<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(s => s);
        }

        public static bool SequenceMatching<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Order().SequenceEqual(second.Order());
        }
    }
}