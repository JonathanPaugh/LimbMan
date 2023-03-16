using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_Poly : GuiButtonBehaviour
	{
        protected override bool OnUse(bool value)
        {
            if (!value) { return false; }
            EditorGUIUtility.PingObject(Game.CreatePoly().gameObject);
            return false;
        }
	}
}