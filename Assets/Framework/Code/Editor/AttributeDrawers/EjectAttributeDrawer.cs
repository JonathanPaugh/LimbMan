using Jape;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

namespace JapeEditor
{
    public class EjectAttributeDrawer : AttributeDrawer<EjectAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            if (label != null) { EditorGUILayout.PrefixLabel(label, new GUIStyle(), new GUIStyle(GUI.skin.label).FontStyle(FontStyle.Bold)); }

            InspectorProperty current = Property.NextProperty();

            GUIHelper.PushIndentLevel(1);

            while (current.IsChildOf(Property))
            {
                current.Draw();
                current = current.NextProperty(false);
                if (current == null) { break; }
            }

            GUIHelper.PopIndentLevel();
        }
    }
}