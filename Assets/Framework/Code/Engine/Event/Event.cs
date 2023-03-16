using System;
using System.Collections;

namespace Jape
{
    public class Event : ISelfRestrictor<Event, object>
    {
        private event Action Handler = delegate{};

        protected Restrictor<object> restrictor = new();

        public Event Subscribe(Action action) 
        {
            Handler += action;
            return this;
        }

        public Event Unsubscribe(Action action) 
        {
            Handler -= action;
            return this;
        }

        public Event Trigger()
        {
            if (restrictor.IsRestricted()) { return this; }
            Handler?.Invoke();
            return this;
        }

        public Event Reset()
        {
            Handler = delegate{};
            restrictor = new Restrictor<object>();
            return this;
        }

        public bool IsRestricted() => restrictor.IsRestricted();

        public Event Restrict(object restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public Event Unrestrict(object restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }

        public IEnumerator Wait()
        {
            Trigger trigger = new();
            Handler += trigger.Invoke;
            yield return trigger.Wait();
            Handler -= trigger.Invoke;
        }
    }

    public class Event<T> : ISelfRestrictor<Event<T>, object>
    {
        private event Action<T> Handler = delegate{};

        protected Restrictor<object> restrictor = new();

        public bool IsRestricted() => restrictor.IsRestricted();

        public Event<T> Subscribe(Action<T> action) 
        {
            Handler += action;
            return this;
        }

        public Event<T> Unsubscribe(Action<T> action) 
        {
            Handler -= action;
            return this;
        }

        public Event<T> Trigger(T args)
        {
            if (restrictor.IsRestricted()) { return this; }
            Handler?.Invoke(args);
            return this;
        }

        public Event<T> Reset()
        {
            Handler = delegate{};
            restrictor = new Restrictor<object>();
            return this;
        }

        public Event<T> Restrict(object restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public Event<T> Unrestrict(object restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }

        public IEnumerator Wait()
        {
            Trigger trigger = new();
            Handler += Invoke;
            yield return trigger.Wait();
            Handler -= Invoke;

           void Invoke(T _) => trigger.Invoke();
        }
    }
}
