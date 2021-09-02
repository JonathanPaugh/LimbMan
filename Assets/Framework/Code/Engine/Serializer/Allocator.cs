using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public class Allocator<T> where T : new()
    {
        public int Max { get; set; }

        private Dictionary<T, bool> items = new Dictionary<T, bool>();

        private Action<T> onCatch;
        private Action<T> onRelease;

        public Allocator(Action<T> onCatch = null, Action<T> onRelease = null)
        {
            this.onCatch = onCatch;
            this.onRelease = onRelease;
        }

        public T Catch()
        {
            foreach (KeyValuePair<T, bool> item in items)
            {
                if (item.Value) { continue; }
                onCatch?.Invoke(item.Key);
                return item.Key;
            }

            T value = new T();
            items.Add(value, true);
            return value;
        }

        public void Release(T value)
        {
            if (!items.ContainsKey(value))
            {
                Log.Warning("Unable to release, adapter does not contain this value");
                return;
            }

            if (items.Count > Max)
            {
                items.Remove(value);
            }

            onRelease?.Invoke(value);
        }

        public void Shrink()
        {
            foreach (KeyValuePair<T, bool> item in items)
            {
                if (item.Value) { continue; }
                items.Remove(item.Key);
            }
        }

        public void Expand()
        {
            while (items.Count < Max)
            {
                items.Add(new T(), false);
            }
        }
    }
}