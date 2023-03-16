using Jape;
using UnityEngine;

namespace JapeNet
{
	public class SyncedInstance
    {
        public enum State { Default, Spawned, Despawned }

        private State state;

        private GameObject instance;
        public GameObject Instance
        {
            get => instance;
            private set
            {
                if (value != null)
                {
                    sceneIndex = value.scene.buildIndex;
                    key = value.Identifier();
                    if (string.IsNullOrEmpty(key))
                    {
                        this.Log().Warning($"Invalid Key: {value}");
                    }
                }
                instance = value;
            }
        }

        public GameObject Prefab { get; }

        private int sceneIndex;
        public int SceneIndex => Instance == null ? sceneIndex : instance.scene.buildIndex;

        private string key;
        public string Key => Instance == null ? key : instance.Identifier();

        public State GetState() { return state; }
        public void SetState(State state) { this.state = state; }

        internal SyncedInstance(State state, GameObject instance, GameObject prefab)
        {
            this.state = state;

            Instance = instance;
            Prefab = prefab;
        }
    }
}