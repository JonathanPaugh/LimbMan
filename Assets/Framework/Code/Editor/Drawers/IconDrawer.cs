using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector.Editor;

namespace JapeEditor
{
    [DrawerPriority(1)]
    public class IconDrawer : Drawer<Icon>
    {
        protected override void Draw(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();

            if (label != null) { EditorGUILayout.PrefixLabel(label, GUIStyle.none, new GUIStyle(GUI.skin.label)); }

            Texture2D texture = (Texture2D)EditorGUILayout.ObjectField(ValueEntry.SmartValue.Texture, typeof(Texture2D), false);
            Color color = EditorGUILayout.ColorField(ValueEntry.SmartValue.Color, GUILayout.MaxWidth(36));

            EditorGUILayout.EndHorizontal();

            if (ValueEntry.SmartValue.Texture != texture)
            {
                ValueEntry.SmartValue = new Icon(texture, ValueEntry.SmartValue.Color);
            }

            if (ValueEntry.SmartValue.Color != color)
            {
                ValueEntry.SmartValue = new Icon(ValueEntry.SmartValue.Texture, color);
            }
        }
    }
}