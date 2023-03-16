using System;
using System.Collections;

namespace Jape
{
    public sealed class Condition : JobDriven<Condition>
    {
        public enum Mode { Single, Repeat }

        private Mode mode;

        private Func<bool> requirement;
        private Action action;

        internal Condition() { Init(this); }

        public override Condition ForceStart()
        {
            if (requirement == null || action == null) { this.Log().Response("Cant start because the condition is not set"); return this; }
            return base.ForceStart();
        }

        public Condition Set(Func<bool> requirement, Action action)
        {
            this.requirement = requirement;
            this.action = action;
            return this;
        }

        public Condition ChangeMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Single: 
                    this.mode = Mode.Single; 
                    ChangeMode(Job.Mode.Single); 
                    break;

                case Mode.Repeat: 
                    this.mode = Mode.Repeat; 
                    ChangeMode(Job.Mode.Loop); 
                    break;
            }
            return this;
        }

        protected override IEnumerable Run()
        {
            yield return Wait.Until(requirement);

            action.Invoke();

            Iteration();
            Complete();
            Processed();

            if (mode == Mode.Repeat && requirement()) { yield return Wait.While(requirement); }
        }
    }
}
