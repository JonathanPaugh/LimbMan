using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using UnityEditor.VersionControl;

namespace JapeEditor
{
	public static class Build
    {
        private static float minCheckWait = 3;
        private static float lastTimeChecked;
        private static bool cachedReadonlyVal = true;

        public static bool IsReadOnly()
        {
            var curTime = Jape.Time.RealtimeCount();
            var timeSinceLastCheck = curTime - lastTimeChecked;

            if (!(timeSinceLastCheck > minCheckWait)) { return cachedReadonlyVal; }

            lastTimeChecked = curTime;
            cachedReadonlyVal = QueryBuildSettingsStatus();

            return cachedReadonlyVal;
        }

        private static bool QueryBuildSettingsStatus()
        {
            if (!Provider.enabled) { return false; }
            if (!Provider.hasCheckoutSupport) { return false; }

            var status = Provider.Status("ProjectSettings/EditorBuildSettings.asset", false);
            status.Wait();

            if (status.assetList == null || status.assetList.Count != 1) { return true; }

            return !status.assetList[0].IsState(Asset.States.CheckedOutLocal);
        }

        public static Scene GetBuildScene(Object sceneObject)
        {
            var entry = new Scene
            {
                buildIndex = -1,
                assetGUID = new GUID(string.Empty)
            };

            if (sceneObject as SceneAsset == null) { return entry; }

            entry.assetPath = AssetDatabase.GetAssetPath(sceneObject);
            entry.assetGUID = new GUID(AssetDatabase.AssetPathToGUID(entry.assetPath));

            var scenes = EditorBuildSettings.scenes;
            for (var index = 0; index < scenes.Length; ++index)
            {
                if (!entry.assetGUID.Equals(scenes[index].guid)) { continue; }

                entry.scene = scenes[index];
                entry.buildIndex = index;
                return entry;
            }

            return entry;
        }

        public static void SetBuildSceneState(Scene buildScene, bool enabled)
        {
            var modified = false;
            var scenesToModify = EditorBuildSettings.scenes;
            foreach (var curScene in scenesToModify.Where(curScene => curScene.guid.Equals(buildScene.assetGUID)))
            {
                curScene.enabled = enabled;
                modified = true;
                break;
            }
            if (modified)
            {
                EditorBuildSettings.scenes = scenesToModify;
            }
        }

        public static void AddBuildScene(Scene buildScene, bool force = false, bool enabled = true)
        {
            if (force == false)
            {
                var selection = EditorUtility.DisplayDialogComplex("Add Scene To Build",
                                                                   "You are about to add scene at " + buildScene.assetPath + " to build settings.",
                                                                   "Enable",
                                                                   "Disable",
                                                                   "Cancel");

                switch (selection)
                {
                    case 0:
                        enabled = true;
                        break;
                    case 1:
                        enabled = false;
                        break;
                    default:
                        return;
                }
            }

            var newScene = new EditorBuildSettingsScene(buildScene.assetGUID, enabled);
            var tempScenes = EditorBuildSettings.scenes.ToList();
            tempScenes.Add(newScene);
            EditorBuildSettings.scenes = tempScenes.ToArray();
        }

        public static void RemoveBuildScene(Scene buildScene, bool force = false)
        {
            var onlyDisable = false;
            if (force == false)
            {
                var selection = -1;

                var title = "Remove Scene From Build";

                var details = "You are about to remove the following scene from build settings:" +
                              System.Environment.NewLine +
                              $"    {buildScene.assetPath}" +
                              System.Environment.NewLine +
                              $"    buildIndex: {buildScene.buildIndex}" +
                              System.Environment.NewLine +
                              System.Environment.NewLine +
                              "This will modify build settings, but the scene asset will remain untouched";

                var confirm = "Remove";
                var alt = "Disable";
                var cancel = "Cancel";

                if (buildScene.scene.enabled)
                {
                    details += "\n\nIf you want, you can just disable the scene";
                    selection = EditorUtility.DisplayDialogComplex(title, details, confirm, alt, cancel);
                }
                else
                {
                    selection = EditorUtility.DisplayDialog(title, details, confirm, cancel) ? 0 : 2;
                }

                switch (selection)
                {
                    case 0:
                        break;
                    case 1:
                        onlyDisable = true;
                        break;
                    default:
                        return;
                }
            }

            if (onlyDisable)
            {
                SetBuildSceneState(buildScene, false);
            }
            else
            {
                var tempScenes = EditorBuildSettings.scenes.ToList();
                tempScenes.RemoveAll(scene => scene.guid.Equals(buildScene.assetGUID));
                EditorBuildSettings.scenes = tempScenes.ToArray();
            }
        }

        public static void OpenSettings()
        {
            EditorWindow.GetWindow(typeof(BuildPlayerWindow));
        }

        public struct Scene
        {
            public int buildIndex;
            public EditorBuildSettingsScene scene;
            public GUID assetGUID;
            public string assetPath;
        }
    }
}