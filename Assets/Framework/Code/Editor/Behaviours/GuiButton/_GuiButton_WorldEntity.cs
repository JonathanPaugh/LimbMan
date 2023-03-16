using Jape;

namespace JapeEditor
{
	public class _GuiButton_WorldEntity : GuiButtonBehaviour
	{
        protected override Evaluation Requirements() 
        { 
            return new Evaluation(
                EntityWindow.ValidateWorldEntity
            );
        }
        protected override bool OnUse(bool value)
        {
            if (!value) { return false; }
            EntityWindow.Open(Entity.Class.World);
            return false;
        }
	}
}