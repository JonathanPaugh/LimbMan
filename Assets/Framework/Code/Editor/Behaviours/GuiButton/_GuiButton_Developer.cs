using UnityEngine;
using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_Developer : GuiButtonBehaviour
	{
        protected override bool OnUse(bool value)
        {
            if (Framework.Settings.developerMode == value) { return value; }
            Framework.Settings.developerMode = value;
            Framework.Settings.Save();
            return value;
        }
	}
}