using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
    [ExecuteAlways]
	public class Mono : MonoBehaviour
    {
        private bool activated;
        private bool alive;

        protected virtual Filter Filter => null;

        public virtual int Player
        {
            get => gameObject.Player();
            set => gameObject.SetPlayer(value);
        }

        public virtual Team Team
        {
            get => gameObject.Team();
            set => gameObject.SetTeam(value);
        }

        public void ApplyForce(Vector3 force, ForceMode mode, bool useMass = true)
        {
            gameObject.ApplyForce(force, mode, useMass);
        }
        
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
        /// Called before Init() when first created, called before Enabled() when set to active
        /// Used to set up values when the object becomes active in any case
        /// </summary>
        protected virtual void Activated() {}

        /// <summary>
        /// Called when initialized, called once in lifetime
        /// Used to set up values and references
        /// </summary>
        protected virtual void Init() {}
        protected virtual void InitEditor() {}

        /// <summary>
        /// Called after scene is loaded, called once in lifetime
        /// </summary>
        protected virtual void InitScene() {}

        /// <summary>
        /// Called after everything is initialized, called once in lifetime
        /// Used to interact with others after element is initialized
        /// </summary>
        protected virtual void First() {}
        protected virtual void FirstEditor() {}

        /// <summary>
        /// Called before destroyed
        /// </summary>
        protected virtual void Destroyed() {}
        protected virtual void DestroyedEditor() {}

        /// <summary>
        /// Called every frame
        /// </summary>
        protected virtual void Frame() {}
        protected virtual void FrameEditor() {}

        /// <summary>
        /// Called every frame after all Frame() calls are done
        /// </summary>
        protected virtual void Late() {}

        /// <summary>
        /// Called in varying amounts every frame depending on physics rate and performance
        /// </summary>
        protected virtual void Tick() {}
        
        /// <summary>
        /// Called to draw GUI
        /// </summary>
        protected virtual void Draw() {}

        /// <summary>
        /// Called when loaded in editor, use to set default inspector values
        /// </summary>
        protected virtual void Default() {}

        /// <summary>
        /// Called when loaded in editor or if modified from inspector
        /// </summary>
        protected virtual void Validated() {}

        /// <summary>
        /// Called when cloned in editor
        /// </summary>
        protected virtual void Cloned() {} 

        /// <summary>
        /// Called on first frame that colliders touch, using Filter
        /// </summary>
        protected virtual void Touch(GameObject gameObject) {}
        internal virtual void TouchLow(GameObject gameObject) {}

        /// <summary>
        /// Called on every frame after first that colliders touch, using Filter
        /// </summary>
        protected virtual void Stay(GameObject gameObject) {}
        internal virtual void StayLow(GameObject gameObject) {}

        /// <summary>
        /// Called on frame that colliders stop touching, using Filter
        /// </summary>
        protected virtual void Leave(GameObject gameObject) {}
        internal virtual void LeaveLow(GameObject gameObject) {}

        /// <summary>
        /// Called before scene is changed
        /// </summary>
        protected virtual void SceneChange(Map map) {}

        /// <summary>
        /// Called after scene is destroyed
        /// </summary>
        protected virtual void SceneDestroyed(Map map) {}

        /// <summary>
        /// Called before scene is created
        /// </summary>
        protected virtual void SceneSetup(Map map) {}

        /// <summary>
        /// Called after scene is created
        /// </summary>
        protected virtual void SceneCreate(Map map) {}

        /// <summary>
        /// Called after scene is loaded
        /// </summary>
        protected virtual void SceneLoad(Map map) {}

        internal virtual void OnEnable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.update += ValidateEditor;
            #endif

            if (Game.IsRunning)
            {
                if (!alive) { return; }
                if (!activated) { activated = true; Activated(); }
                Enabled();
            }
            else
            {
                EnabledEditor();
            }
        } 
        
        internal virtual void OnDisable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= ValidateEditor;
            #endif

            if (Game.IsRunning)
            {
                if (!alive) { return; }
                Disabled();
                activated = false;
            }
            else { DisabledEditor(); }
        }

        private void ValidateEditor()
        {
            // ReSharper disable HeuristicUnreachableCode
            if (this == null) { return; }
            // ReSharper restore HeuristicUnreachableCode
            if (Game.IsRunning) { return; }
            FrameEditor();

        }

        internal virtual void Awake()
        {
            if (Game.IsRunning)
            {
                if (alive) { return; }
                alive = true;

                if (!activated) { activated = true; Activated(); }

                Init();

                if (!GetType().IsGenericSubclassOf(typeof(Manager<>)))
                {
                    EngineManager.Instance.runtimeMono.Add(this);
                }
            }
            else
            {
                switch (UnityEngine.Event.current?.commandName)
                {
                    case "Paste":
                    case "Duplicate":
                        Cloned();
                        break;
                }
                InitEditor();
            }
        }

        private void ValidateInitScene(Map map)
        {
            EngineManager.Instance.OnSceneLoad -= ValidateInitScene;
            InitScene();
        }
        
        internal virtual void Start()
        {
            if (Game.IsRunning)
            {
                EngineManager.Instance.OnSceneChange += SceneChange;
                EngineManager.Instance.OnSceneDestroy += SceneDestroyed;
                EngineManager.Instance.OnSceneSetup += SceneSetup;
                EngineManager.Instance.OnSceneCreate += SceneCreate;
                EngineManager.Instance.OnSceneLoad += SceneLoad;
                EngineManager.Instance.OnSceneLoad += ValidateInitScene;
                First();
            }
            else { FirstEditor(); }
        }

        internal virtual void OnDestroy()
        {
            if (Game.IsRunning)
            {
                if (!alive) { return; }
                alive = false;

                if (EngineManager.Instance != null)
                {
                    if (!GetType().IsGenericSubclassOf(typeof(Manager<>)))
                    {
                        EngineManager.Instance.runtimeMono.Remove(this);
                    }

                    EngineManager.Instance.OnSceneChange -= SceneChange;
                    EngineManager.Instance.OnSceneDestroy -= SceneDestroyed;
                    EngineManager.Instance.OnSceneSetup -= SceneSetup;
                    EngineManager.Instance.OnSceneCreate -= SceneCreate;
                    EngineManager.Instance.OnSceneLoad -= SceneLoad;
                    EngineManager.Instance.OnSceneLoad -= ValidateInitScene;
                }

                Destroyed();
            }
            else { DestroyedEditor(); }
        }

        internal virtual void Update()
        {
            if (Game.IsRunning) { Frame(); }
        }

        internal virtual void LateUpdate()
        {
            if (Game.IsRunning) { Late(); }
        }

        internal virtual void FixedUpdate()
        {
            if (Game.IsRunning) { Tick(); }
        }

        internal virtual void OnGUI()
        {
            Draw();
        }

        internal virtual void Reset() { Default(); }

        internal virtual void OnValidate()
        {
            if (Game.IsRunning) { return; }
            Validated();
        }

        internal virtual void OnTriggerEnter(Collider collider)
        {
            if (!Game.IsRunning) { return; }
            TouchLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Touch(collider.gameObject);
        }

        internal virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (!Game.IsRunning) { return; }
            TouchLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Touch(collider.gameObject);
        }

        internal virtual void OnTriggerStay(Collider collider)
        {
            if (!Game.IsRunning) { return; }
            StayLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Stay(collider.gameObject);
        }

        internal virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (!Game.IsRunning) { return; }
            StayLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Stay(collider.gameObject);
        }

        internal virtual void OnTriggerExit(Collider collider)
        {
            if (!Game.IsRunning) { return; }
            LeaveLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Leave(collider.gameObject);
        }

        internal virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (!Game.IsRunning) { return; }
            LeaveLow(collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collider.gameObject)) { return; }
            Leave(collider.gameObject);
        }

        internal virtual void OnCollisionEnter(Collision collision)
        {
            if (!Game.IsRunning) { return; }
            TouchLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Touch(collision.collider.gameObject);
        }

        internal virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!Game.IsRunning) { return; }
            TouchLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Touch(collision.collider.gameObject);
        }

        internal virtual void OnCollisionStay(Collision collision)
        {
            if (!Game.IsRunning) { return; }
            StayLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Stay(collision.collider.gameObject);
        }

        internal virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (!Game.IsRunning) { return; }
            StayLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Stay(collision.collider.gameObject);
        }

        internal virtual void OnCollisionExit(Collision collision)
        {
            if (!Game.IsRunning) { return; }
            LeaveLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Leave(collision.collider.gameObject);
        }

        internal virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!Game.IsRunning) { return; }
            LeaveLow(collision.collider.gameObject);
            if (Filter != null && !Filter.Evaluate(collision.collider.gameObject)) { return; }
            Leave(collision.collider.gameObject);
        }

        [Pure] public static IEnumerable<T> FindAll<T>() where T : Mono { return FindAll(typeof(T)).Cast<T>(); }
        [Pure] public static IEnumerable<Mono> FindAll(Type type) { return FindAll().Where(e => e.GetType().IsBaseOrSubclassOf(type)); }
        [Pure] public static IEnumerable<Mono> FindAll() { return EngineManager.Instance.runtimeMono.Where(m => m.gameObject.activeInHierarchy); }
    }
}