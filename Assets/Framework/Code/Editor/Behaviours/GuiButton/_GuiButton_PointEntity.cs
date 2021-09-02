using UnityEngine;
using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_PointEntity : GuiButtonBehaviour
	{
        protected override bool OnUse(bool value)
        {
            if (!value) { return false; }
            EntityWindow.Open(Entity.Class.Point); 
            return false;
        }
	}
}