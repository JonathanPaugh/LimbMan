using System;
using UnityEngine.SceneManagement;

namespace Jape
{
	public class Cache<T>
    {
        private bool dirty = true;
        private readonly Func<T> instantiate;

        private T value;
        public T Value
        {
            get
            {
                if (!dirty) { return value; }
                dirty = false;
                value = instantiate();
                return value;
            }
            set => this.value = value;
        }

        public Cache(Func<T> instantiate)
        {
            this.instantiate = instantiate;
        }

        public static Cache<T> CreateEditorManaged(Func<T> instantiate)
        {
            Cache<T> cache = new(instantiate);

            #if UNITY_EDITOR
            HookManagedEditor(cache);
            #endif

            return cache;
        }

        private static void HookManagedEditor(Cache<T> cache)
        {
            #if UNITY_EDITOR

            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChange;
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnactiveSceneChangedInEditMode;

            void onPlayModeStateChange(UnityEditor.PlayModeStateChange change)
            {
                if (change != UnityEditor.PlayModeStateChange.EnteredEditMode) { return; }
                cache.SetDirty();
            }

            void EditorSceneManagerOnactiveSceneChangedInEditMode(Scene oldScene, Scene newScene)
            {
                cache.SetDirty();
            }

            #endif
        }

        public void SetDirty() => dirty = true;
    }
}