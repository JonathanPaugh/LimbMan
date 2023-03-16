using Jape;

namespace JapeEditor
{
	public class _GuiButton_Developer : GuiButtonBehaviour
	{
        protected override bool OnUse(bool value)
        {
            if (Framework.DeveloperMode == value) { return value; }
            Framework.DeveloperMode = value;
            return value;
        }
	}
}