using UnityEngine;
using UnityEditor;
using Jape;
using EditorSettings = Jape.EditorSettings;

namespace JapeEditor
{
    [InitializeOnLoad]
	public static class Hierarchy
    {
        private static EditorSettings settings;
        public static EditorSettings Settings => settings != null ? settings : settings = Framework.Settings<EditorSettings>();

        static Hierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
            EditorApplication.quitting += OnEditorQuit;
        }


        private static void OnGUI(int instanceID, Rect rect)
        {
            const int offset = 5;

            Object instance = EditorUtility.InstanceIDToObject(instanceID);

            if (instance is GameObject == false) { return; }
            
            GameObject gameObject = (GameObject)instance;
            
            int childCount = gameObject.transform.childCount;

            if (childCount > 0)
            {
                EditorGUI.LabelField(new Rect(rect.x + offset, rect.y, rect.width, rect.height), childCount.ToString(), new GUIStyle().FontColor(GUI.contentColor)
                                                                                                                                      .Alignment(TextAnchor.MiddleRight)
                                                                                                                                      .FontSize(8));
            }
        }

        private static void OnEditorQuit()
        {
            EditorApplication.quitting -= OnEditorQuit;
            EditorApplication.hierarchyWindowItemOnGUI -= OnGUI;
        }
    }
}