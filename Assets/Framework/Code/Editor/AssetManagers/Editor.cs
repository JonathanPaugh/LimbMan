using UnityEditor;
using UnityEngine;
using Jape;
using EditorSettings = Jape.EditorSettings;

namespace JapeEditor
{
    public partial class Editor : AssetManager<Editor>
    {
        private EditorSettings Settings => Framework.Settings<EditorSettings>();

        public Editor() { Instance = this; }

        protected override void EnabledEditor()
        {
            Initializer.Init();
            Settings.OnExposePackages += SetInternals;
            PrefabUtility.prefabInstanceUpdated += OnPrefabApply;
            EditorApplication.quitting += OnEditorQuit;
        }

        protected override void DisabledEditor()
        {
            EditorApplication.quitting -= OnEditorQuit;
            PrefabUtility.prefabInstanceUpdated -= OnPrefabApply;
            Settings.OnExposePackages -= SetInternals;
        }

        private static void OnPrefabApply(GameObject instance)
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            prefab.Properties().Id = null;
        }

        private static void OnEditorQuit()
        {
            RouterWindow.CloseAll();
            Initializer.Destroy();
        }

        internal void SetInternals(bool visible)
        {
            const string AssemblyFileName = "AssemblyInfo";
            string probuilderPath = IO.JoinPath("Packages", "com.unity.probuilder");
            string spriteshapePath = IO.JoinPath("Packages", "com.unity.2d.spriteshape");

            ModifyAssemblyInfo((TextAsset)AssetDatabase.LoadMainAssetAtPath($"{IO.JoinPath(probuilderPath, "Runtime", "Core", AssemblyFileName)}.cs"), visible);
            ModifyAssemblyInfo((TextAsset)AssetDatabase.LoadMainAssetAtPath($"{IO.JoinPath(probuilderPath, "Editor", "EditorCore", AssemblyFileName)}.cs"), visible);
            ModifyAssemblyInfo((TextAsset)AssetDatabase.LoadMainAssetAtPath($"{IO.JoinPath(spriteshapePath, "Runtime", AssemblyFileName)}.cs"), visible);
        }

        private static void ModifyAssemblyInfo(TextAsset asset, bool visible)
        {
            if (asset == null) { return; }

            const string EngineAttribute = "[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"Jape.Engine\")]";
            const string EditorAttribute = "[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"Jape.Editor\")]";

            FileWriter writer = new();

            if (visible)
            {
                if (!asset.text.Contains(EngineAttribute)) { writer.SetContent($"{asset.text.Trim()}{System.Environment.NewLine}{EngineAttribute}").WriteEditor(AssetDatabase.GetAssetPath(asset)); }
                if (!asset.text.Contains(EditorAttribute)) { writer.SetContent($"{asset.text.Trim()}{System.Environment.NewLine}{EditorAttribute}").WriteEditor(AssetDatabase.GetAssetPath(asset)); }
            }
            else
            {
                if (asset.text.Contains(EngineAttribute)) { writer.SetContent($"{asset.text.Replace(EngineAttribute, string.Empty)}").WriteEditor(AssetDatabase.GetAssetPath(asset)); }
                if (asset.text.Contains(EditorAttribute)) { writer.SetContent($"{asset.text.Replace(EditorAttribute, string.Empty)}").WriteEditor(AssetDatabase.GetAssetPath(asset)); }
            }
        }
    }
}