using System;

namespace Jape
{
    public abstract partial class Element
    {
        public class Receiver<TData> where TData : IReceivable
        {
            private IReceivable receivable;
            private Action<Element, TData> action;

            private bool active = true;

            internal Receiver(Action<Element, TData> action) { this.action = action; }

            public void Receive(Element self, IReceivable receivable)
            {
                if (!active) { return; }
                action?.Invoke(self, (TData)receivable);
            }

            public void Enable()
            {
                active = true;
            }

            public void Disable()
            {
                active = false;
            }
        }
    }
}