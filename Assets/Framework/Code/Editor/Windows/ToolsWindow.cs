using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public class ToolsWindow : Window, IHasCustomMenu
    {
        protected override string Title => "Tools";

        protected override float Width => 64;

        private Vector2 scrollPosition;
        
        [SerializeField]
        [HideIf(nameof(IsSet))]
        private ToolChain toolChain;

        private bool IsSet() => toolChain != null;

        protected override void OnEnable()
        {
            if (toolChain != null) { return; }
            toolChain = DataType.Find<ToolChain>("Default");
        }

        protected override void Draw()
        {
            if (!IsSet()) { return; }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);

            toolChain.Draw(Width);

            GUILayout.EndScrollView();

            GUIHelper.RequestRepaint();
        }

        [MenuItem("Window/Tools", false, -100)]
        private static void Menu() { Open<ToolsWindow>(); }

        public void AddItemsToMenu(GenericMenu menu)
        {
            foreach (ToolChain toolChain in DataType.FindAll<ToolChain>())
            {
                menu.AddItem(new GUIContent(IO.JoinPath("Toolchains", toolChain.name)), false, () =>
                {
                    this.toolChain = toolChain;
                });
            }
        }
    }
}