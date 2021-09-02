using System;
using System.Collections.Generic;

namespace Jape
{
	public abstract class ModifierInstance<T> : ModifierInstance where T : Element
    {
        protected T Target { get; private set; }
        protected T Inflictor { get; private set; }

        protected Timer lifetime = Timer.Create();

        private bool active;

        internal void Init(T target, T inflictor, float lifetime, IEnumerable<Behaviour.Arg> args)
        {
            if (active) { return; } active = true;

            Init(args);

            target.AddModifier(this);

            Target = target;
            Inflictor = inflictor;

            if (lifetime > 0) { this.lifetime.CompletedAction(Destroy).Set(lifetime).Start(); }

            OnInflicted();
        }

        public override void Destroy()
        {
            if (!active) { return; } active = false;

            lifetime.Stop();

            OnDestroyed();

            if (Target.HasModifier(this)) { Target.RemoveModifier(this); }
        }

        public static TModifier Inflict<TModifier>(T target, T inflictor, float lifetime, IEnumerable<Behaviour.Arg> args) where TModifier : ModifierInstance<T>
        {
            return (TModifier)Inflict(Modifier.Find<TModifier>(), target, inflictor, lifetime, args);
        }

        public static ModifierInstance<T> Inflict(Modifier modifier, T target, T inflictor, float lifetime, IEnumerable<Behaviour.Arg> args)
        {
            ModifierInstance<T> instance = (ModifierInstance<T>)modifier.CreateInstance();
            instance.Init(target, inflictor, lifetime, args);
            return instance;
        }
    }
}