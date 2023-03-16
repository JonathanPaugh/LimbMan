using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace JapeEditor
{
	public static class Prefab
    {
        public static bool IsOpen() { return GetActive() != null; }

        public static PrefabStage GetActive() { return PrefabStageUtility.GetCurrentPrefabStage(); }

        public static void InsertActive(GameObject gameObject)
        {
            if (!IsOpen()) { return; }
            EditorScene.Transfer(gameObject, GetActive().scene);
            gameObject.transform.SetParent(GetActive().prefabContentsRoot.transform, false);
        }

        public static bool IsPrefab(GameObject gameObject) { return IsAsset(gameObject) || IsInstance(gameObject) || IsStaged(gameObject); }

        public static bool IsRoot(GameObject gameObject)
        { 
            if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject)) { return true; }

            PrefabStage stage = PrefabStageUtility.GetPrefabStage(gameObject);
            return stage != null && stage.prefabContentsRoot == gameObject;
        }

        public static bool IsPartExclusive(GameObject gameObject)
        {
            if (IsRoot(gameObject)) { return false; }
            return PrefabUtility.IsPartOfAnyPrefab(gameObject) ||
                   PrefabStageUtility.GetPrefabStage(gameObject).IsPartOfPrefabContents(gameObject);
        }

        // Possibly implement IsRootExclusive & IsPart if needed in the future //

        public static bool IsAsset(GameObject gameObject)
        {
            if (IsInstance(gameObject)) { return false; }
            return PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab;
        }
        public static bool IsInstance(GameObject gameObject) { return PrefabUtility.GetPrefabInstanceStatus(gameObject) != PrefabInstanceStatus.NotAPrefab; }
        public static bool IsStaged(GameObject gameObject) { return PrefabStageUtility.GetPrefabStage(gameObject) != null; }

        public static GameObject GetRoot(GameObject gameObject)
        {
            if (IsRoot(gameObject)) { return gameObject; }
            if (IsInstance(gameObject)) { return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject); }
            if (IsStaged(gameObject)) { return PrefabStageUtility.GetPrefabStage(gameObject).prefabContentsRoot; }
            return null;  
        }
    }
}