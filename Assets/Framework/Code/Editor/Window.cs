using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace JapeEditor
{
	public abstract class Window : OdinEditorWindow
    {
        private const int OdinDrawSpace = 2;

        protected abstract string Title { get; }

        protected virtual Display DisplayMode => Display.Default;

        protected virtual bool AutoHeight => false;
        protected virtual float MinHeight => 0;

        protected virtual float Width => 256;

        public enum Display { Default, Popup }

        protected virtual void Init() {}
        protected virtual void Draw() {}

        protected sealed override void Initialize()
        {
            base.Initialize();
            UseScrollView = false;
            Init();
        }

        protected sealed override void OnGUI() { base.OnGUI(); }

        protected sealed override void OnBeginDrawEditors()
        {
            if (AutoHeight) { GUIHelper.BeginLayoutMeasuring(); }
            EditorGUI.BeginChangeCheck();
        }

        protected sealed override void OnEndDrawEditors()
        {
            Draw();

            if (!EditorGUI.EndChangeCheck())
            {
                if (AutoHeight)
                {
                    Vector2 size = GUIHelper.EndLayoutMeasuring().size;
                    if (size != Vector2.zero)
                    {
                        minSize = new Vector2(Width, size.y + OdinDrawSpace);
                        maxSize = new Vector2(maxSize.x, minSize.y);
                    }
                } 
                else
                {
                    minSize = new Vector2(Width, MinHeight);
                }
            }
        }

        protected void UpdateTitle() { titleContent = new GUIContent(Title); }

        private static bool IsPopup<T>() where T : Window
        {
            T window = GetWindowWithRect<T>(Rect.zero);
            Display displayType = window.DisplayMode;
            window.Close();
            return displayType == Display.Popup;
        }

        public static T Open<T>() where T : Window
        {
            T window = GetWindow<T>(IsPopup<T>());
            window.UpdateTitle();
            return window;
        }
    }
}