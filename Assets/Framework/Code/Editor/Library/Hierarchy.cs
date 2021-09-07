using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using EditorSettings = Jape.EditorSettings;

namespace JapeEditor
{
    [InitializeOnLoad]
	public static class Hierarchy
    {
        private static Map activeMap;
        private static List<GameObject> activeGameObjects = new List<GameObject>();

        private static EditorSettings settings;
        public static EditorSettings Settings => settings ?? (settings = Jape.Game.Settings<EditorSettings>());

        static Hierarchy()
        {
            EditorApplication.hierarchyChanged += OnChange;
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
            EditorApplication.quitting += OnEditorQuit;
        }

        private static void OnChange()
        {
            if (Jape.Game.IsRunning) { return; }

            if (activeMap != Map.GetActive())
            {
                activeMap = Map.GetActive();
                activeGameObjects.Clear();
            }

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>().Where(g => !activeGameObjects.Contains(g)))
            {
                activeGameObjects.Add(gameObject);

                if (Settings.autoProperty)
                {
                    if (!gameObject.HasComponent<Properties>(false))
                    {
                        Properties.Create(gameObject);
                    }
                }
            }
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
            EditorApplication.hierarchyChanged -= OnChange;
            EditorApplication.hierarchyWindowItemOnGUI -= OnGUI;
        }
    }
}