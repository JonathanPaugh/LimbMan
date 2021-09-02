using System;
using System.Linq;
using UnityEngine;

namespace Jape
{
    public abstract class Manager<T> : Mono where T : Manager<T>
    {
        protected static bool InitOnLoad => false;
        protected virtual Type[] Components => null;

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

                CreateInstance();

                return instance;
            }
            set { instance = value; }
        }

        public static bool HasInstance()
        {
            return instance != null;
        }

        public static bool IsQuitting()
        {
            return isQuitting;
        }

        protected static void CreateInstance()
        {
            instance = new GameObject(null, typeof(T)).GetComponent<T>();
            instance.gameObject.name = instance.GetType().Name;

            if (instance.Components != null)
            {
                foreach (Type component in instance.Components)
                {
                    instance.gameObject.AddComponent(component);
                }
            }

            if (Application.isPlaying) { DontDestroyOnLoad(instance.gameObject); }
        }

        protected static T Get() { return FindObjectOfType<T>(true); }
	
        protected virtual void PreQuit() {} 

        private void OnApplicationQuit()
        {
            PreQuit();
            isQuitting = true;
            Destroy();
            Destroy(gameObject);
        }

        bool queueDestroy;
        protected override void Late()
        {
            if (queueDestroy)
            {
                queueDestroy = false;
                Destroyed();
            }
        }

        protected virtual void PreDestroy() {} 

        private void Destroy()
        {
            queueDestroy = true;
            PreDestroy();
        }

        protected override void Destroyed()
        {
            instance = null;
        }
    }
}