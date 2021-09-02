using System;

namespace Jape
{
	public abstract class Module<T> : Module, IModule<T> where T : Module
    {
        [NonSerialized] 
        protected T instance;

        /// <summary>
        /// Need to find new implementation for this, Would be good not to have Init() methods
        /// </summary>
        internal void Init(T instance) { this.instance = instance; }

        protected override void DestroyAction() { Stop(); }

        public virtual T Start()
        {
            if (processing) { this.Log().Response("Cant start because it is processing, use ForceStart() to override"); return instance; }

            ForceStart();

            return instance;
        }

        public virtual T ForceStart()
        {
            if (processing) { Stop(); }

            onStart.Trigger(this, EventArgs.Empty);

            processing = true;
            paused = false;
            complete = false;

            StartAction();

            return instance;
        }

        public virtual T Stop()
        {
            if (!processing) { return instance; }

            onStop.Trigger(this, EventArgs.Empty);

            StopAction();

            if (ProcessOnStop) { Processed(); }

            return instance;
        }

        public virtual T Pause()
        {
            if (paused || !processing) { return instance; }

            onPause.Trigger(this, EventArgs.Empty);

            paused = true;

            PauseAction();

            return instance;
        }

        public virtual T Resume()
        {
            if (!paused || !processing) { return instance; }

            onResume.Trigger(this, EventArgs.Empty);

            paused = false;

            ResumeAction();

            return instance;
        }

        public static T Create()
        {
            T module = (T)Activator.CreateInstance(typeof(T), true);
            ModuleManager.Instance.Add(module);
            return module;
        }

        public static T CreateGlobal()
        {
            T module = (T)Activator.CreateInstance(typeof(T), true);
            ModuleManager.Instance.AddGlobal(module);
            return module;
        }

        public static void DestroyGlobal(T module)
        {
            ModuleManager.Instance.DestroyGlobal(module);
        }
    }
}