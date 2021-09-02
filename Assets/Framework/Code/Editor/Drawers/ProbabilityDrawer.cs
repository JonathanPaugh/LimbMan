using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;

namespace JapeEditor
{
    [DrawerPriority(1)]
    public class ProbabilityDrawer : PercentDrawer<Probability>
    {
        protected override Probability DrawField(Rect position, float value)
        {
            return EditorGUI.Slider(position.AlignLeft(position.width), value, 0, 100);
        }
    }
}