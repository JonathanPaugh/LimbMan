using Jape;
using UnityEngine;

namespace GameEditor
{
	public class _GuiButton_CreateTrigger : GuiButtonBehaviour
	{
		protected override bool OnUse(bool value)
		{
			if (!value) { return false; }
            JapeEditor.Game.CloneGameObject(Database.GetAsset("Trigger").Load<GameObject>());
			return false;
		}
	}
}