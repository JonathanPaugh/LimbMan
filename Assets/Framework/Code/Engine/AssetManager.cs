using UnityEngine;

namespace Jape
{
    public abstract class AssetManager<T> : Scriptable where T : AssetManager<T>
    {
        private static bool isQuitting;

        private static T instance;

        public static T Instance 
        {
            get
            {
                if (isQuitting) { return null; }

                if (instance != null) { return instance; }

                instance = Get();

                if (instance != null) { return instance; }

                instance = CreateInstance<T>();

                #if UNITY_EDITOR

                string path = "Assets/Managers/Resources";

                Directory.CreateFileFolders(path);
                UnityEditor.AssetDatabase.CreateAsset(instance, $"{path}/{typeof(T).ToString().RemoveNamespace()}.asset");

                #endif

                return instance;
            }
            set { instance = value; }
        }

        private static T Get() { return Resources.Load<T>(typeof(T).CleanName()); }

        internal override void OnEnable()
        {
            base.OnEnable();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += Destroy;
            #endif
        }

        internal override void OnDisable()
        {
            base.OnDisable();

            Destroy();
        }

        private static void Destroy()
        {
            instance = null;
            isQuitting = true;
        }
    }
}