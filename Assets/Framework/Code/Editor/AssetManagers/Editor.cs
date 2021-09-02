using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Jape;

namespace JapeEditor
{
    public partial class Editor : AssetManager<Editor>
    {
        [SerializeField] 
        [HideInInspector]
        private bool exposePackages = false;

        public Editor() { Instance = this; }

        protected override void EnabledEditor()
        {
            Initializer.Init();
            PrefabUtility.prefabInstanceUpdated += OnPrefabApply;
            EditorApplication.quitting += OnEditorQuit;
        }

        protected override void DisabledEditor()
        {
            EditorApplication.quitting -= OnEditorQuit;
            PrefabUtility.prefabInstanceUpdated -= OnPrefabApply;
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
    }
}