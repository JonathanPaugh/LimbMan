using System.Collections.Generic;

namespace Jape
{
    public static class QueueExt
    {
        public static IEnumerable<T> DequeueRange<T>(this Queue<T> queue, int count) 
        {
            for (int i = 0; i < count && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}