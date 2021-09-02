using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;

namespace JapeEditor
{
	public class _GuiButton_Collider : GuiButtonBehaviour
	{
        protected override Evaluation Requirements() { return new Evaluation (() => Selection.gameObjects.Any(World.IsWorld)); }

        protected override bool OnUse(bool value)
        {
            if (!value) { return false; }
            foreach (GameObject gameObject in Selection.gameObjects.Where(World.IsWorld)) { World.MakeCollider(gameObject); }
            return false;
        }
	}
}