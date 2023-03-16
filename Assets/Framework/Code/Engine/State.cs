using System;
using System.Collections.Generic;

namespace Jape
{
    public class State : ISelfRestrictor<State, object>
    {
        protected bool value;
        protected bool defaultValue;

        protected List<Action> trueActions = new();
        protected List<Action> falseActions = new();

        protected Restrictor<object> restrictor = new();

        public State(bool defaultValue = false)
        {
            this.defaultValue = value;
            value = defaultValue;
        }

        public bool Default() { return defaultValue; }
        public bool IsRestricted() => restrictor.IsRestricted();

        public State Set(bool value)
        {
            if (this.value == value) { return this; }
            if (IsRestricted()) { return this; }
                 
            this.value = value;
            LaunchActions(value);

            return this;
        }

        public State Toggle()
        {
            Set(!value);
            return this;
        }

        public State Reset()
        {
            trueActions = new List<Action>();
            falseActions = new List<Action>();

            restrictor = new Restrictor<object>();

            value = defaultValue;

            return this;
        }

        public State Restrict(object restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public State Unrestrict(object restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }

        public State AttachAction(Action action, bool onValue)
        {
            if (onValue) { trueActions.Add(action); }
            else { falseActions.Add(action); }

            return this;
        }

        public State DetachAction(Action action, bool onValue)
        {
            if (onValue) { trueActions.Remove(action); }
            else { falseActions.Remove(action); }

            return this;
        }

        private void LaunchActions(bool onValue)
        {
            Action[] actions = onValue ? trueActions.ToArray() : falseActions.ToArray();
            foreach (Action action in actions) { action?.Invoke(); }
        }

        public static implicit operator bool(State state) { return state != null && state.value; }

        public override string ToString() { return value.ToString(); }
    }
}
