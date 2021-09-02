using UnityEngine;
using UnityEditor;
using Jape;
using UnityEditor.Experimental.SceneManagement;

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