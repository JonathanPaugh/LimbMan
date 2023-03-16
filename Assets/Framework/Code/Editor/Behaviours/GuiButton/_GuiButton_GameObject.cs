using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_GameObject : GuiButtonBehaviour
	{
		protected override bool OnUse(bool value) 
        {
            if (!value) { return false; }
            EditorGUIUtility.PingObject(Game.EmptyGameObject());
            return false;
        }
    }
}