using System;
using System.Collections;
using UnityEngine.Events;

namespace Jape
{
    public class Event
    {
        public event Action<object> Handler = delegate{};

        protected Restrictor<Type> restrictor = new Restrictor<Type>();

        public Event Trigger(object sender)
        {
            if (restrictor.IsRestricted()) { return this; }
            Handler?.Invoke(sender);
            return this;
        }

        public Event Reset()
        {
            Handler = delegate{};
            restrictor = new Restrictor<Type>();
            return this;
        }

        public Event Restrict(Type restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public Event Unrestrict(Type restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }

        public IEnumerator Wait()
        {
            Trigger trigger = new Trigger();
            Handler += trigger.Invoke;
            yield return trigger.Wait();
            Handler -= trigger.Invoke;
        }
    }

    public class Event<T>
    {
        public event EventHandler<T> Handler = delegate{};

        protected Restrictor<Type> restrictor = new Restrictor<Type>();

        public Event<T> Trigger(object sender, T args)
        {
            if (restrictor.IsRestricted()) { return this; }
            Handler?.Invoke(sender, args);
            return this;
        }

        public Event<T> Reset()
        {
            Handler = delegate{};
            restrictor = new Restrictor<Type>();
            return this;
        }

        public Event<T> Restrict(Type restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public Event<T> Unrestrict(Type restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }

        public IEnumerator Wait()
        {
            Trigger<T> trigger = new Trigger<T>();
            Handler += trigger.Invoke;
            yield return trigger.Wait();
            Handler -= trigger.Invoke;
        }
    }
}
