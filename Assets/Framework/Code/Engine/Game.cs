using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

using Object = UnityEngine.Object;

namespace Jape
{
	public class Game
    {
        public static Camera DefaultCamera => Find<Camera>().FirstOrDefault();

        public static Sprite DefaultSprite => Database.GetAsset<Texture2D>("SpriteDefault100").Load<Sprite>();
        public static SpriteShape DefaultSpriteShape => Database.GetAsset<SpriteShape>("Default").Load<SpriteShape>();
        public static Texture DefaultTexture => Database.GetAsset<Texture2D>("TextureDefault100").Load<Texture2D>();
        public static Material DefaultMaterial => Database.GetAsset<Material>("Default").Load<Material>();
        public static Material DefaultMaterial2D => Database.GetAsset<Material>("Default2D").Load<Material>();

        public const string GameIsRunning = "@Game.IsRunning";

        public static bool IsRunning => Application.isPlaying;
        public static bool IsBuild => !Application.isEditor;
        public static bool IsWeb => Application.platform == RuntimePlatform.WebGLPlayer;
        public static bool IsAOT => IsWeb || Application.platform == RuntimePlatform.IPhonePlayer;
        public static bool IsMobile
        {
            get
            {
                if (IsWeb)
                {
                    return WebManager.IsMobile();
                }

                return Application.isMobilePlatform;
            }
        }

        public static Scene ActiveScene() { return SceneManager.GetActiveScene(); }

        public static void Quit()
        {
            if (!IsRunning) { return; }

            #if UNITY_EDITOR
            if (!IsBuild)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            #endif

            Application.Quit();
        }

        public static GameObject CreateGameObject(string name = null, params Type[] components)
        {
            GameObject gameObject = new(name, components.ToArray());
            Properties.Create(gameObject);
            if (Application.isPlaying) { gameObject.AddTag(Tag.Find("Instantiated")); }
            return gameObject;
        }

        public static T CreateEntity<T>() where T : Entity { return CreateGameObject(typeof(T).ToString().RemoveNamespace().Replace("Ent", string.Empty), typeof(T)).GetComponent<T>(); }

        public static GameObject CloneGameObject(GameObject gameObject, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            GameObject clone;
            if (position != null && rotation != null) { clone = Object.Instantiate(gameObject, (Vector3)position, (Quaternion)rotation, parent); }
            else if (position != null) { clone = Object.Instantiate(gameObject, (Vector3)position, gameObject.transform.rotation, parent); }
            else if (rotation != null) { clone = Object.Instantiate(gameObject, gameObject.transform.position, (Quaternion)rotation, parent); }
            else { clone = Object.Instantiate(gameObject, parent); }

            clone.name = gameObject.name;
            if (IsRunning) { clone.AddTag(Tag.Find("Instantiated")); }

            return clone;
        }

        public static GameObject CloneGameObjectInactive(GameObject gameObject, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            bool active = gameObject.activeSelf;
            gameObject.SetActive(false);
            GameObject clone = CloneGameObject(gameObject, position, rotation, parent);
            gameObject.SetActive(active);
            return clone;
        }

        public static void ChangeScene(string scenePath, string loadingScreenPath, Action onChange = null)
        {
            EngineManager.ChangeScene(scenePath, loadingScreenPath, onChange);
        }

        public static void ChangeScene(string scenePath, Action onChange = null)
        {
            EngineManager.ChangeScene(scenePath, null, onChange);
        }

        public static void Save(Action onSave = null)
        {
            EngineManager.SaveGame(onSave);
        }

        public static void Load(Action<bool> onLoad = null)
        {
            EngineManager.LoadGame(onLoad);
        }

        public static void Delete(Action onDelete = null)
        {
            EngineManager.DeleteGame(onDelete);
        }

        protected static float pauseTimescale = -1;
        public static void Pause()
        {
            if (pauseTimescale > -1)
            {
                Log.Warning("Game is already paused");
                return;
            }

            pauseTimescale = UnityEngine.Time.timeScale;

            UnityEngine.Time.timeScale = 0f;
        }

        public static void Resume()
        {
            if (pauseTimescale < 0)
            {
                Log.Warning("Game is not paused");
                return;
            }

            UnityEngine.Time.timeScale = pauseTimescale;

            pauseTimescale = -1;
        }

        /// <summary>
        /// Find object of type in scene
        /// </summary>
        [Pure]
        public static IEnumerable<Object> Find(Type type)
        {
            if (!IsRunning) { return Object.FindObjectsOfType(type, true); }
            return Object.FindObjectsOfType(type, true);
        }

        /// <summary>
        /// Find object of type in scene
        /// </summary>
        [Pure]
        public static IEnumerable<T> Find<T>() where T : Object
        {
            return Find(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Find active object of type in scene
        /// </summary>
        [Pure]
        public static IEnumerable<Object> FindActive(Type type)
        {
            if (!IsRunning) { return Object.FindObjectsOfType(type); }
            if (type.IsBaseOrSubclassOf(typeof(Element))) { return Element.FindAll(type); }
            if (type.IsBaseOrSubclassOf(typeof(Mono))) { return Mono.FindAll(type); }
            if (type.IsBaseOrSubclassOf(typeof(Scriptable))) { return Scriptable.FindAll(type); }
            return Object.FindObjectsOfType(type);
        }

        /// <summary>
        /// Find active object of type in scene
        /// </summary>
        [Pure]
        public static IEnumerable<T> FindActive<T>() where T : Object
        {
            return FindActive(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Find object of type in scene or assets
        /// </summary>
        public static IEnumerable<Object> FindResource(Type type)
        {
            return Find(type).Concat(Database.GetAssets(null, type, true).Select(r => r.Load()));
        }

        /// <summary>
        /// Find object of type in scene or assets
        /// </summary>
        public static IEnumerable<T> FindResource<T>() where T : Object
        {
            return FindResource(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Find object of type from unity, very expensive
        /// </summary>
        public static IEnumerable<Object> FindDeep(Type type)
        {
            return Resources.FindObjectsOfTypeAll(type);
        }

        /// <summary>
        /// Find object of type from unity, very expensive
        /// </summary>
        public static IEnumerable<T> FindDeep<T>() where T : Object
        {
            return FindDeep(typeof(T)).Cast<T>();
        }
    }
}