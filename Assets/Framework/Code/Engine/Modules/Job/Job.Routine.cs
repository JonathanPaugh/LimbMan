using System;
using System.Collections;

namespace Jape
{
    public partial class Job
    {
        public class Routine
        {
            private IEnumerable enumerable;
            private Action action;

            internal Routine(IEnumerable enumerable) { this.enumerable = enumerable; }
            internal Routine(Action action) { this.action = action; }

            public Type Type() { return enumerable != null ? typeof(IEnumerable) : typeof(Action); }

            internal IEnumerator SetEnumerator() { return enumerable.GetEnumerator(); }

            public object Get()
            {
                if (Type() == typeof(IEnumerable))
                {
                    return enumerable;
                }

                if (Type() == typeof(Action))
                {
                    return action;
                }

                return null;
            }

            internal void Launch(Action enumeration, Action action)
            {
                if (Type() == typeof(IEnumerable)) { enumeration?.Invoke(); }
                if (Type() == typeof(Action)) { action?.Invoke(); }
            }
        }
    }
}