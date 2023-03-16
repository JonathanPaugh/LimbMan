using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace JapeEditor
{
	public static class EditorScene
    {
        public static Scene GetActive() { return Prefab.IsOpen() ? Prefab.GetActive().scene : GetLoaded(); }
        public static Scene GetLoaded() { return SceneManager.GetActiveScene(); }

        public static void MarkDirty() { EditorSceneManager.MarkSceneDirty(GetActive()); }

        public static void Transfer(GameObject gameObject, Scene scene) { SceneManager.MoveGameObjectToScene(gameObject, scene); }
    }
}