using Jape;
using UnityEngine;

namespace GameEditor
{
	public class _GuiButton_CreateDeath : GuiButtonBehaviour
	{
		protected override bool OnUse(bool value)
		{
			if (!value) { return false; }
            JapeEditor.Game.ClonePrefab(Database.GetAsset("Death").Load<GameObject>());
			return false;
		}
	}
}