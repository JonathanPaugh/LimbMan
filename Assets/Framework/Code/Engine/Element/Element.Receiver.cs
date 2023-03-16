using System;

namespace Jape
{
    public abstract partial class Element
    {
        public interface IReceiver
        {
            bool Active { get; set; }
            Type Type { get; }

            void Receive(IReceivable receivable);
        }

        public class Receiver<T> : IReceiver where T : IReceivable
        {
            public bool Active { get; set; } = true;
            public Type Type => typeof(T);

            private readonly Action<T> action;

            internal Receiver(Action<T> action) { this.action = action; }

            public void Receive(IReceivable receivable)
            {
                if (!Active) { return; }
                action?.Invoke((T)receivable);
            }

            public void Enable()
            {
                Active = true;
            }

            public void Disable()
            {
                Active = false;
            }
        }
    }
}