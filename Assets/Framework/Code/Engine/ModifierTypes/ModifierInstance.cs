namespace Jape
{
	public abstract class ModifierInstance : BehaviourInstance
    {
        public abstract void Destroy();

        protected abstract void OnInflicted();
        protected abstract void OnDestroyed();
    }
}