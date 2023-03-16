using System;
using System.Collections.Generic;
using Jape;

namespace JapeNet
{
	public class NetDelegator
    {
        private Dictionary<string, Delegate> delegates = new();

        public void Add(string key, Action<object[]> action) { delegates.Add(key, new Delegate(action)); }
        public void Remove(string key) { delegates.Remove(key); }

        public Delegate Get(string key)
        {
            if (!delegates.TryGetValue(key, out Delegate @delegate))
            {
                Log.Write($"Could not find delegate: {key}");
                return null;
            }

            return @delegate;
        }

        public void Invoke(string key, object[] args)
        {
            if (!delegates.TryGetValue(key, out Delegate @delegate))
            {
                Log.Write($"Could not find delegate: {key}");
                return;
            }

            @delegate.Invoke(args);
        }

        public class Delegate
        {
            private Action<object[]> action;
            private bool active;

            internal Delegate(Action<object[]> action)
            {
                this.action = action;
            }

            internal void Invoke(object[] args)
            {
                if (!active) { return; }
                action.Invoke(args);
            }

            public Delegate Enable() { active = true; return this; }
            public Delegate Disable() { active = false; return this; }
        }
    }
}