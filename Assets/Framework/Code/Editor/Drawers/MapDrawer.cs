using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor.VersionControl;

namespace JapeEditor
{
    public class MapDrawer : Drawer<Map>
    {
        protected override void Draw(GUIContent label) 
        {
            Rect position = EditorGUILayout.GetControlRect();

            if (label != null) { position = EditorGUI.PrefixLabel(position, label); }

            DrawField();

            GUILayout.Space(-2);

            DrawInfo();

            void DrawField()
            {
                ValueEntry.SmartValue.sceneAsset = EditorGUI.ObjectField(position, ValueEntry.SmartValue.sceneAsset, typeof(SceneAsset), false);
                ValueEntry.SmartValue.ScenePath = ValueEntry.SmartValue.GetScenePathFromAsset();
            }

            void DrawInfo()
            {
                Build.Scene buildScene = Build.GetBuildScene(ValueEntry.SmartValue.sceneAsset);

                bool readOnly = Build.IsReadOnly();
                string readOnlyWarning = readOnly ? System.Environment.NewLine +
                                                    System.Environment.NewLine +
                                                    "WARNING: Build Settings is not checked out and so cannot be modified" : "";

                GUIContent labelContent = new GUIContent();

                if (buildScene.buildIndex == -1)
                {
                    labelContent.text = "NOT In Build";
                    labelContent.tooltip = "This scene is NOT in build settings" +
                                           System.Environment.NewLine +
                                           "It will be NOT included in builds";
                }
                else if (buildScene.scene.enabled)
                {
                    labelContent.text = "BuildIndex: " + buildScene.buildIndex;
                    labelContent.tooltip = "This scene is in build settings and ENABLED" +
                                           System.Environment.NewLine +
                                           "It will be included in builds" + readOnlyWarning;
                }
                else
                {
                    labelContent.text = "BuildIndex: " + buildScene.buildIndex;
                    labelContent.tooltip = "This scene is in build settings and DISABLED" +
                                           System.Environment.NewLine +
                                           "It will be NOT included in builds";
                }

                GUILayout.BeginHorizontal();

                if (readOnly) { GUIHelper.PushGUIEnabled(false); }

                if (buildScene.buildIndex == -1)
                {
                    int index = EditorBuildSettings.scenes.Length;
                    if (GUILayout.Button($"Add ({index})", EditorStyles.miniButtonLeft)) { Build.AddBuildScene(buildScene); }
                }
                else
                {
                    int index = buildScene.buildIndex;
                    bool enabled = buildScene.scene.enabled;
                    string state = enabled ? "Disable" : "Enable";
                    if (GUILayout.Button(state, EditorStyles.miniButtonLeft)) { Build.SetBuildSceneState(buildScene, !enabled); }
                    if (GUILayout.Button($"Remove ({index})", EditorStyles.miniButtonMid)) { Build.RemoveBuildScene(buildScene, !enabled); }
                }

                if (readOnly) { GUIHelper.PopGUIEnabled(); }

                if (GUILayout.Button("Settings", EditorStyles.miniButtonRight)) { Build.OpenSettings(); }

                GUILayout.EndHorizontal();
            }
        }
    }
}