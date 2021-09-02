using System;
using Jape;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

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

    [SerializeField] public Object sceneAsset;

    private bool IsValidSceneAsset
    {
        get
        {
            if (!sceneAsset) { return false; }
            return sceneAsset is SceneAsset;
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
            return GetScenePathFromAsset();
            #else
            return scenePath;
            #endif
        }
        set
        {
            scenePath = value;
            #if UNITY_EDITOR
            sceneAsset = GetSceneAssetFromPath();
            #endif
        }
    }

    public bool IsSame(Map map) { return scenePath == map.scenePath; }
    public bool IsSet() { return !string.IsNullOrEmpty(scenePath); }

    public static void Change(Map map, Map loadingScreen = null)
    {
        Game.ChangeScene(map.scenePath, loadingScreen.scenePath);
    }

    public static Map GetActive()
    {
        return new Map(Game.ActiveScene().path);
    }

    #if UNITY_EDITOR

    internal void Open()
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }

    internal string GetSceneName()
    {
        if (sceneAsset == null) { return string.Empty; }
        return sceneAsset.name;
    }

    internal SceneAsset GetSceneAssetFromPath()
    {
        return string.IsNullOrEmpty(scenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }

    internal string GetScenePathFromAsset()
    {
        return sceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(sceneAsset);
    }

    private void HandleBeforeSerialize()
    {
        if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
        {
            sceneAsset = GetSceneAssetFromPath();
            if (sceneAsset == null) { scenePath = string.Empty; }
            EditorSceneManager.MarkAllScenesDirty();
        }
        else
        {
            scenePath = GetScenePathFromAsset();
        }
    }

    private void HandleAfterDeserialize()
    {
        EditorApplication.update -= HandleAfterDeserialize;
        if (IsValidSceneAsset) { return; }
        if (string.IsNullOrEmpty(scenePath)) { return; }
        sceneAsset = GetSceneAssetFromPath();
        if (!sceneAsset) { scenePath = string.Empty; }
        if (!Application.isPlaying) { EditorSceneManager.MarkAllScenesDirty(); }
    }

    #endif

    internal void Refresh()
    {
        #if UNITY_EDITOR
        scenePath = GetScenePathFromAsset();
        #endif
    }

    public void OnBeforeSerialize()
    {
        #if UNITY_EDITOR
        HandleBeforeSerialize();
        #endif
    }

    public void OnAfterDeserialize()
    {
        #if UNITY_EDITOR
        EditorApplication.update += HandleAfterDeserialize;
        #endif
    }

    public static implicit operator string(Map map) { return map.ScenePath; }
}