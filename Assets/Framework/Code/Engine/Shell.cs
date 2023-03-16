namespace Jape
{
	public abstract class Shell : Mono
    {
        internal override void Awake()
        {
            base.Awake();
            if (Game.IsRunning) { Subscribe(); }
        }

        internal override void OnDestroy()
        {
            base.OnDestroy();
            if (Game.IsRunning) { Unsubscribe(); }
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
    }
}