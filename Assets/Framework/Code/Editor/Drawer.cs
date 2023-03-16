using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace JapeEditor
{
	[DrawerPriority(-1)]
	public class Drawer<T> : OdinValueDrawer<T>
    {
        protected virtual void Draw(GUIContent label) {}
        protected sealed override void DrawPropertyLayout(GUIContent label) { Draw(label); }
    }
}