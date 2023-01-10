using System;

namespace Jape
{
	public abstract class Module
    {
        protected virtual bool ProcessOnStop => true;

        protected bool processing;
        protected bool paused;
        protected bool complete;
        protected bool destroyed;

        public Event<EventArgs> onStart = new Event<EventArgs>();
        public Event<EventArgs> onStop = new Event<EventArgs>();
        public Event<EventArgs> onPause = new Event<EventArgs>();
        public Event<EventArgs> onResume = new Event<EventArgs>();
        public Event<EventArgs> onIteration = new Event<EventArgs>();
        public Event<EventArgs> onProcessed = new Event<EventArgs>();
        public Event<EventArgs> onComplete = new Event<EventArgs>();

        internal void Destroy()
        {
            if (destroyed) { return; }
            destroyed = true;
            DestroyAction();
        }

        protected abstract void StartAction();
        protected abstract void StopAction();
        protected abstract void PauseAction();
        protected abstract void ResumeAction();
        protected abstract void DestroyAction();

        protected virtual void Iteration() { onIteration.Trigger(this, EventArgs.Empty); }
        protected virtual void Processed() { processing = false; paused = false; onProcessed.Trigger(this, EventArgs.Empty); }
        protected virtual void Complete() { complete = true; onComplete.Trigger(this, EventArgs.Empty); }
        
        public bool IsProcessing() { return processing; }
        public bool IsRunning() { return processing && !paused; }
        public bool IsPaused() { return paused; }
        public bool IsComplete() { return complete; }

        public static bool IsAlive(Module module) { return module != null && module.IsProcessing(); }

        public object WaitIdle()
        {
            if (processing) { return onProcessed.Wait(); }
            return new Wait.Skip();
        }
    }
}