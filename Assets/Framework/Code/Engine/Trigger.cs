namespace Jape
{
    public class Trigger
    {
        protected bool triggered;

        public void Invoke() { triggered = true; }

        public object Wait() { return Jape.Wait.Until(() => triggered); }
    }
}
