using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [ExecuteAlways]
	public class Scriptable : SerializedScriptableObject
    {
        private bool alive;

        /// <summary>
        /// Called when enabled
        /// </summary>
        protected virtual void Enabled() {}
        protected virtual void EnabledEditor() {}

        /// <summary>
        /// Called when disabled
        /// </summary>
        protected virtual void Disabled() {}
        protected virtual void DisabledEditor() {}

        /// <summary>
        /// Called when initialized, called once in lifetime
        /// </summary>
        protected virtual void Init() {}
        protected virtual void InitEditor() {}

        /// <summary>
        /// Called before destroyed
        /// </summary>
        protected virtual void Destroyed() {}
        protected virtual void DestroyedEditor() {}

        /// <summary>
        /// Called when loaded in editor, use to set default inspector values
        /// </summary>
        protected virtual void Default() {}

        /// <summary>
        /// Called when loaded in editor or if modified from inspector
        /// </summary>
        protected virtual void Validated() {}

        protected void SaveEditor()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }

        internal virtual void OnEnable()
        {
            if (Game.IsRunning) { Enabled(); }
            else { EnabledEditor(); }
        }

        internal virtual void OnDisable()
        {
            if (Game.IsRunning) { Disabled(); }
            else { DisabledEditor(); }
        }

        internal virtual void Awake()
        {
            if (Game.IsRunning)
            {
                if (alive) { return; }
                alive = true;

                Init();

                if (!GetType().IsGenericSubclassOf(typeof(AssetManager<>)))
                {
                    EngineManager.Instance.runtimeScriptable.Add(this);
                }
            }
            else { InitEditor(); }
        }

        internal virtual void OnDestroy()
        {
            if (Game.IsRunning)
            {
                if (!alive) { return; }
                alive = false;

                if (!GetType().IsGenericSubclassOf(typeof(AssetManager<>)))
                {
                    EngineManager.Instance.runtimeScriptable.Remove(this);
                }

                Destroyed();
            }
            else { DestroyedEditor(); }
        }

        internal virtual void Reset() { Default(); }

        internal virtual void OnValidate() { Validated(); }

        [Pure] public static IEnumerable<T> FindAll<T>() where T : Scriptable { return FindAll(typeof(T)).Cast<T>(); }
        [Pure] public static IEnumerable<Scriptable> FindAll(Type type) { return FindAll().Where(e => e.GetType().IsBaseOrSubclassOf(type)); }
        [Pure] public static IEnumerable<Scriptable> FindAll() { return EngineManager.Instance.runtimeScriptable; }
    }
}