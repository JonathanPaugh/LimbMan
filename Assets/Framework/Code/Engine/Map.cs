using System;
using Jape;
using UnityEngine;

using Object = UnityEngine.Object;

[Serializable]
public class Map : ISerializationCallbackReceiver
{
    internal Map() {}
    public Map(string scenePath)
    {
        ScenePath = scenePath;
    }

    #if UNITY_EDITOR

    [SerializeField] 
    public Object sceneAsset;

    private bool IsValidSceneAsset
    {
        get
        {
            if (!sceneAsset) { return false; }
            return sceneAsset is UnityEditor.SceneAsset;
        }
    }

    #endif

    [SerializeField]
    private string scenePath = string.Empty;

    public string ScenePath
    {
        get
        {
            #if UNITY_EDITOR
            return GetScenePathFromAssetEditor();
            #else
            return scenePath;
            #endif
        }
        set
        {
            scenePath = value;
            #if UNITY_EDITOR
            sceneAsset = GetSceneAssetFromPathEditor();
            #endif
        }
    }

    public bool IsSame(Map map) { return scenePath == map.scenePath; }
    public bool IsSet() { return !string.IsNullOrEmpty(scenePath); }

    public static void Change(Map map, Map loadingScreen = null)
    {
        Game.ChangeScene(map.scenePath, loadingScreen?.scenePath);
    }

    public static Map GetActive()
    {
        return new Map(Game.ActiveScene().path);
    }

    internal string GetSceneNameEditor()
    {
        #if UNITY_EDITOR
        if (sceneAsset == null) { return string.Empty; }
        return sceneAsset.name;
        #else
        return null;
        #endif
    }

    internal void OpenEditor()
    {
        #if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
        #endif
    }

    #if UNITY_EDITOR
    internal UnityEditor.SceneAsset GetSceneAssetFromPathEditor()
    {
        return string.IsNullOrEmpty(scenePath) ? null : UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(scenePath);
    }
    #endif

    #if UNITY_EDITOR
    internal string GetScenePathFromAssetEditor()
    {
        return sceneAsset == null ? string.Empty : UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
    }
    #endif

    private void HandleBeforeSerializeEditor()
    {
        #if UNITY_EDITOR

        if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
        {
            sceneAsset = GetSceneAssetFromPathEditor();
            if (sceneAsset == null) { scenePath = string.Empty; }
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        else
        {
            scenePath = GetScenePathFromAssetEditor();
        }

        #endif
    }

    private void HandleAfterDeserializeEditor()
    {
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.update -= HandleAfterDeserializeEditor;
        if (IsValidSceneAsset) { return; }
        if (string.IsNullOrEmpty(scenePath)) { return; }
        sceneAsset = GetSceneAssetFromPathEditor();
        if (!sceneAsset) { scenePath = string.Empty; }
        if (!Application.isPlaying) { UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty(); }

        #endif
    }

    internal void RefreshEditor()
    {
        #if UNITY_EDITOR
        scenePath = GetScenePathFromAssetEditor();
        #endif
    }

    public void OnBeforeSerialize()
    {
        #if UNITY_EDITOR
        HandleBeforeSerializeEditor();
        #endif
    }

    public void OnAfterDeserialize()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.update += HandleAfterDeserializeEditor;
        #endif
    }

    public static implicit operator string(Map map) { return map.ScenePath; }
}