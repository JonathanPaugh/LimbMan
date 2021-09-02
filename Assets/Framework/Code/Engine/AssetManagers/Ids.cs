using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
    public class Ids : AssetManager<Ids>
    {
        public Ids() { Instance = this; }

        [PropertySpace(8)]

        [SerializeField]
        private List<Id> ids = new List<Id>();

        private Job rebuildJob = new EditorJob();


        [PropertyOrder(-1)]
        [Button(ButtonSizes.Large)]
        private void Rebuild()
        {
            #if UNITY_EDITOR

            if (Module.IsAlive(rebuildJob)) { return; }

            Verify();
            RefreshMaps();

            rebuildJob.Set(Routine()).Start();

            IEnumerable Routine()
            {
                Trigger trigger = new Trigger();

                List<Map> maps = ids.Select(i => i.Map).
                                 GroupBy(m => m.ScenePath).
                                 Select(g => g.First()).
                                 Where(m => !m.IsSame(Map.GetActive())).
                                 ToList();

                maps.Add(Map.GetActive());

                ids.Clear();

                foreach (Map map in maps)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += Trigger;

                    map.Open();

                    yield return trigger.Wait();

                    UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= Trigger;

                    yield return Wait.Editor();
                }

                void Trigger(Scene Scene, UnityEditor.SceneManagement.OpenSceneMode Mode) { trigger.Invoke(this); }
            }

            #endif
        }

        public static IEnumerable<Id> GetAll()
        {
            RefreshMaps();
            return Instance.ids;
        }


        public static Id Get(string id)
        {
            RefreshMaps();
            return Instance.ids.FirstOrDefault(i => i.Value == id);
        }

        public static bool Has(string id) { return Instance.ids.Any(i => i.Value == id); }
        public static bool Has(Properties properties) { return Instance.ids.Where(i => i.Properties != null).Any(i => i.Properties == properties); }

        private static void Verify() { Instance.ids.RemoveAll(i => !i.Map.IsSet()); }
        private static void RefreshMaps() { foreach (Id id in Instance.ids) { id.Map.Refresh(); }}
        
        internal static void Add(Properties properties) { Instance?.ids.Add(new Id(properties)); }
        internal static void Remove(Properties properties) { Instance?.ids.RemoveAll(i => i.Value == properties.Id); }
        internal static void Refresh(Properties properties)
        {
            if (Has(properties)) { return; }
            Id id = GetAll().First(i => i.Properties == properties);
            id.Properties = properties;
        }

        private static void Save()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(Instance);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        protected override void EnabledEditor() { Verify(); }
        protected override void DisabledEditor() { rebuildJob.Stop(); }

        internal override void OnEnable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += Save;
            #endif

            base.OnEnable();
        }

        public static string Generate()
        {
            string temp;
            do
            {
                temp = Guid.NewGuid().ToString();
                
            } while (Match() || MatchRuntime());
            return temp;

            bool Match() { return Ids.GetAll().Any(i => i.Value == temp); }
            bool MatchRuntime()
            {
                if (!Game.IsRunning) { return false; } 
                if (Properties.runtimeIds.None(i => i == temp)) { return false; }
                return true;
            }
        }

        [Serializable]
        public class Id
        {
            public Id(Properties properties)
            {
                Properties = properties;
            }

            #if UNITY_EDITOR
            [HideLabel]
            [ShowInInspector]
            private string display => $"{map.GetSceneName()} - {value}";
            #endif

            [SerializeField, HideInInspector]
            private Properties properties;
            public Properties Properties
            {
                get => properties;
                internal set
                {
                    #if UNITY_EDITOR
                    properties = value;
                    position = value.transform.position;
                    this.value = value.Id;
                    name = value.name;
                    Map = Map.GetActive();
                    #endif
                }
            }

            [SerializeField, HideInInspector]
            private Vector3 position;
            public Vector3 Position 
            { 
                get => position;
                private set => position = value;
            }

            [SerializeField, HideInInspector]
            private string value;
            public string Value 
            { 
                get => value;
                private set => this.value = value;
            }

            [SerializeField, HideInInspector]
            private string name;
            public string Name
            {
                get => name;
                private set => name = value;
            }

            [SerializeField, HideInInspector]
            private Map map;
            public Map Map 
            { 
                get => map; 
                private set => map = value;
            }
        }
    }
}