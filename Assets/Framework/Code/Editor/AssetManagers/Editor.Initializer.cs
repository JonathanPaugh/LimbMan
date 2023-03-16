using UnityEditor;
using UnityEngine;
using Jape;
using UnityEditor.SceneManagement;

namespace JapeEditor
{
    public partial class Editor
    {
        private static class Initializer
        {
            static Initializer()
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode) { Init(); }
                EditorApplication.playModeStateChanged += Validate;
                EditorSceneManager.sceneOpened += delegate { Init(); };
            }

            public static void Init()
            {
                OnLoad();
                EditorApplication.update += LoadScene;
            }

            private static void LoadScene()
            {
                EditorApplication.update -= LoadScene;
                OnLoadScene();
            }

            public static void OnLoad()
            {
                Instance.SetInternals(Instance.Settings.ExposePackages);
            }

            public static void OnLoadScene() {} 
            
            public static void Destroy() { EditorApplication.playModeStateChanged -= Validate; }

            private static void Validate(PlayModeStateChange state)
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredEditMode: Init(); break;
                    default: return;
                }
            }
        }
    }
}