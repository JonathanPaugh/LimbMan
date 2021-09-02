using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_Sprite : GuiButtonBehaviour
	{
		protected override bool OnUse(bool value)
		{
			if (!value) { return false; }
            EditorGUIUtility.PingObject(Game.CreateSprite().gameObject);
			return false;
		}
	}
}