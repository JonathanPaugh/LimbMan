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

        public Event onStart = new();
        public Event onStop = new();
        public Event onPause = new();
        public Event onResume = new();
        public Event onIteration = new();
        public Event onProcessed = new();
        public Event onComplete = new();

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

        protected virtual void Iteration() { onIteration.Trigger(); }
        protected virtual void Processed() { processing = false; paused = false; onProcessed.Trigger(); }
        protected virtual void Complete() { complete = true; onComplete.Trigger(); }
        
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