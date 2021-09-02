using System;

namespace Jape
{
    public class Trigger
    {
        protected bool triggered;

        public void Invoke(object sender) { triggered = true; }

        public object Wait() { return Jape.Wait.Until(() => triggered); }
    }

    public class Trigger<T>
    {
        protected bool triggered;

        public void Invoke(object sender, T args) { triggered = true; }

        public object Wait() { return Jape.Wait.Until(() => triggered); }
    }
}
