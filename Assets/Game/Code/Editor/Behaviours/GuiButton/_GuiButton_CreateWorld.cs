using UnityEditor;
using Jape;
using JapeEditor;
using UnityEngine;

namespace GameEditor
{
	public class _GuiButton_CreateWorld : GuiButtonBehaviour
	{
		protected override bool OnUse(bool value)
		{
			if (!value) { return false; }
            JapeEditor.Game.CloneGameObject(Database.GetAsset("World").Load<GameObject>());
			return false;
		}
	}
}