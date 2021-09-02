using UnityEngine;
using UnityEditor;
using Jape;
using UnityEngine.U2D;

namespace JapeEditor
{
	public class _GuiButton_Shape : GuiButtonBehaviour
	{
        protected override bool OnUse(bool value)
        {
            if (!value) { return false; }
            EditorGUIUtility.PingObject(Game.CreateShape().gameObject);
            return false;
        }
	}
}