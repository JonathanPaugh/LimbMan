using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Jape;

namespace JapeEditor
{
    public class PercentDrawer<T> : Drawer<T> where T : Percent
    {
        private GUIContent symbol = new("%");
        private float SymbolWidth => GUI.skin.label.CalcSize(symbol).x;

        protected override void Draw(GUIContent label)
        {
            Rect position = EditorGUILayout.GetControlRect();

            if (label != null) { position = EditorGUI.PrefixLabel(position, label); }

            T value = ValueEntry.SmartValue;

            value = DrawField(position, value.Display());

            EditorGUI.LabelField(position.AlignRight(SymbolWidth), symbol);

            if (!Mathf.Approximately(ValueEntry.SmartValue, value)) { ValueEntry.SmartValue = value; }
        }

        protected virtual T DrawField(Rect position, float value)
        {
            return (T)EditorGUI.FloatField(position.AlignLeft(position.width), value);
        }
    }
}