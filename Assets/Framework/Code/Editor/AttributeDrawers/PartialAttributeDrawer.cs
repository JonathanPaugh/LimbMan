using System;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
	public class PartialAttributeDrawer : AttributeDrawer<PartialAttribute>
	{
        private ValueResolver<string> textResolver;

        protected override void Initialize()
        {
            textResolver = ValueResolver.GetForString(Property, Attribute.Selector);
        }

        public override bool CanDrawTypeFilter(Type type) { return type == typeof(string); }

        protected override void Draw(GUIContent label) 
		{
            string text = textResolver.GetValue();

            GUILayout.BeginHorizontal();

            if (label != null) { EditorGUILayout.PrefixLabel(label); }

            Rect position = GUILayoutUtility.GetRect(new GUIContent(text), GUI.skin.label);

            EditorGUI.LabelField(position, string.Empty, GUI.skin.textField);

            GUIHelper.PushGUIEnabled(false);

            EditorGUI.LabelField(position, text, GUI.skin.label);

            GUIHelper.PopGUIEnabled();

            Property.ValueEntry.WeakSmartValue = EditorGUI.TextField(position.AddX(GUI.skin.label.CalcSize(new GUIContent(text)).x - 4), (string)Property.ValueEntry.WeakSmartValue, GUI.skin.label);

            GUILayout.EndHorizontal();
		}
	}
}