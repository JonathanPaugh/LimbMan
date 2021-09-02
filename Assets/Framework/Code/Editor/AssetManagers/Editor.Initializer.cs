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
                EditorApplication.update += OnLoadScene;
            }

            private static void LoadScene()
            {
                EditorApplication.update -= OnLoadScene;
                OnLoadScene();
            }

            public static void OnLoad()
            {
                ModifyInternals((TextAsset)AssetDatabase.LoadMainAssetAtPath("Packages/com.unity.probuilder/Runtime/Core/AssemblyInfo.cs"));
                ModifyInternals((TextAsset)AssetDatabase.LoadMainAssetAtPath("Packages/com.unity.probuilder/Editor/EditorCore/AssemblyInfo.cs"));
                ModifyInternals((TextAsset)AssetDatabase.LoadMainAssetAtPath("Packages/com.unity.2d.spriteshape/Runtime/AssemblyInfo.cs"));
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

            private const string EngineAttribute = "[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"Jape.Engine\")]";
            private const string EditorAttribute = "[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"Jape.Editor\")]";

            private static void ModifyInternals(TextAsset asset)
            {
                if (asset == null) { return; }

                FileWriter writer = new FileWriter();

                if (Instance.exposePackages)
                {
                    if (!asset.text.Contains(EngineAttribute)) { writer.SetContent($"{asset.text.Trim()}{System.Environment.NewLine}{EngineAttribute}").Write(AssetDatabase.GetAssetPath(asset)); }
                    if (!asset.text.Contains(EditorAttribute)) { writer.SetContent($"{asset.text.Trim()}{System.Environment.NewLine}{EditorAttribute}").Write(AssetDatabase.GetAssetPath(asset)); }
                }
                else
                {
                    if (asset.text.Contains(EngineAttribute)) { writer.SetContent($"{asset.text.Replace(EngineAttribute, string.Empty)}").Write(AssetDatabase.GetAssetPath(asset)); }
                    if (asset.text.Contains(EditorAttribute)) { writer.SetContent($"{asset.text.Replace(EditorAttribute, string.Empty)}").Write(AssetDatabase.GetAssetPath(asset)); }
                }
            }
        }
    }
}