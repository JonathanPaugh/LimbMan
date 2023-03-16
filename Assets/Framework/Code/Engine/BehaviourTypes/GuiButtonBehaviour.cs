namespace Jape
{
	public abstract class GuiButtonBehaviour : BehaviourInstance
    {
        protected virtual Evaluation Requirements() { return null; }
        public bool Enabled() { return Requirements() ?? true; }

        protected abstract bool OnUse(bool value);

        public bool Use(bool value) { return OnUse(value); }
    }
}