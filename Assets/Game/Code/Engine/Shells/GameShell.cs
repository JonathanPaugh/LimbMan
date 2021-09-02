using System;
using System.Linq;
using Jape;
using UnityEngine;

namespace Game
{
	public class GameShell : Jape.GameShell
    {
        private static Player player;

        protected override void OnBuildInit() {}
        protected override void OnGameInit()
        {
            Jape.Game.Load(loaded =>
            {
                if (!loaded)
                {
                    string scene;
                    scene = Jape.Game.IsBuild ? Database.GetAsset<Level>("Area1").Load<Level>().Map : Jape.Game.ActiveScene().name;
                    Jape.Game.ChangeScene(scene);
                    Setup(Create);
                }
            });

            void Create()
            {
                CreatePlayer();
                MoveToSpawn(null);
            }
        }

        protected override void OnGameLoad()
        {
            Spawn();
        }

        protected override void OnSceneChange(Map map) {}
        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}

        protected override void OnSceneCreate(Map map)
        {
            MoveCameraToPlayer();
        }

        protected override void OnSceneLoad(Map map) {} 
        
        private static void Setup(Action onSetup)
        {
            EngineManager.Instance.OnSceneCreate += OnSetup;

            void OnSetup(Map map)
            {
                EngineManager.Instance.OnSceneCreate -= OnSetup;
                onSetup?.Invoke();
            }
        }

        public static void CreatePlayer()
        {
            if (player != null) { Destroy(player.gameObject); }
            player = Jape.Game.CloneGameObject(Database.LoadPrefab("Player")).GetComponent<Player>();
            DontDestroyOnLoad(player);
        }

        public static void Spawn()
        {
            string id = null;
            string scene = Jape.Game.ActiveScene().path;

            if (SaveManager.PullStatus("Spawn", out Status status))
            {
                id = (string)status.Read("Id");
                scene = (string)status.Read("Scene");
            }

            Jape.Game.ChangeScene(scene);
            Setup(() =>
            {
                CreatePlayer();
                MoveToSpawn(id);
            });
        }

        public static void MoveCameraToPlayer()
        {
            if (player == null) { return; }
            if (Jape.Game.DefaultCamera == null) { return; }

            Jape.Game.DefaultCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Jape.Game.DefaultCamera.transform.position.z);
        }

        public static void MoveToSpawn(string id)
        {
            if (player == null) { return; }
            Spawn spawn = GetSpawn(id);
            Vector3 position = spawn.SpawnPosition();
            player.transform.position = position;
            MoveCameraToPlayer();
        }

        private static Spawn GetSpawn(string id = null)
        {
            Spawn[] spawns = Element.FindAll<Spawn>().ToArray();
            Spawn spawn = null;
            
            if (id != null)
            {
                spawn = spawns.FirstOrDefault(s => s.gameObject.Id() == id);
            }

            if (spawn == null)
            {
                spawn = spawns.FirstOrDefault(s => s.primary);
            }

            if (spawn == null)
            {
                spawn = spawns.FirstOrDefault();
            }

            return spawn;
        }
    }
}