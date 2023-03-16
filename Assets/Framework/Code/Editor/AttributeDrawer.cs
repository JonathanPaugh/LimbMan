using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace JapeEditor
{
	[DrawerPriority(-1)]
	public class AttributeDrawer<T> : OdinAttributeDrawer<T> where T : Attribute
    {
        protected virtual void Draw(GUIContent label) {}

        protected sealed override void DrawPropertyLayout(GUIContent label) { Draw(label); }
    }
}