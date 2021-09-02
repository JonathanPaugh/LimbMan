using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public class ToolsWindow : Window
    {
        protected override string Title => "Tools";

        protected override float Width => 70;

        protected override void Draw()
        {
            foreach (ToolButton button in ToolButton.GetOrder()) { button.Draw(); }

            GUILayout.Space(8);

            GUIHelper.RequestRepaint();
        }

        [MenuItem("Window/Tools", false, -100)]
        private static void Menu() { Open<ToolsWindow>(); }
    }
}