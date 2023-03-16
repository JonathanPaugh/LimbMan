using System;
using System.Linq;
using Jape;
using UnityEngine;

using Time = Jape.Time;

namespace Game
{
    public sealed class GameManager : Manager<GameManager>
    {
        private new static bool InitOnLoad => true;

        public Player Player { get; private set; }
        public UI UI { get; private set; }
        public SpeedTimer Timer { get; private set; }

        public new static void Init()
        {
            Camera camera = Game.CloneGameObject(Database.LoadPrefab("Camera")).GetComponent<Camera>();
            DontDestroyOnLoad(camera);

            Instance.UI = UI.Create(camera);
            Instance.Timer = new SpeedTimer();
            Instance.Timer.Log().ToggleDiagnostics();
            Jape.Game.Load(loaded =>
            {
                SaveManager.Instance.AutoSave.Start(1f, true);
                if (loaded)
                {
                    LoadGame();
                } 
                else
                {
                    NewGame();
                }
            });
        }

        private static void NewGame()
        {
            string scene = Jape.Game.IsBuild ? Game.Settings.startingLevel.Map : Jape.Game.ActiveScene().name;
            Jape.Game.ChangeScene(scene);
            Setup(Create);

            static void Create()
            {
                CreatePlayer();
                MoveToSpawn(null);
                Instance.Timer.Start();
            }
        }

        private static void LoadGame()
        {
            float time = 0;

            if (SaveManager.PullStatus("Time", out Status statusTime))
            {
                time = (float)statusTime.Read("Current");
            }

            if (SaveManager.PullStatus("Records", out Status statusRecords))
            {
                Instance.UI.SetRecord((float)statusRecords.Read("Best"));
            }

            Setup(() =>
            {
                Instance.Timer.Start(time);
            });
            Spawn();
        }

        public static void Restart()
        {
            SaveManager.RemoveStatus("Spawn");
            SaveManager.RemoveStatus("Time");
            NewGame();
        }

        public static void Finish()
        {
            float time = Instance.Timer.Stop();

            if (!SaveManager.PullStatus("Records", out Status status))
            {
                status = new Status
                {
                    Key = "Records"
                };
            }

            if (status.Has("Best"))
            {
                float best = (float)status.Read("Best");
                if (best <= time)
                {
                    time = best;
                }
            }

            status.Write("Best", time);

            Status.Save(status);

            Instance.UI.SetRecord(time);


            UnityEngine.Time.timeScale = 0.1f;
            Jape.Timer.Delay(5f, Time.Counter.Realtime, () =>
            {
                UnityEngine.Time.timeScale = 1f;
                NewGame();
            });
        }

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
            Player player = Instance.Player;
            if (player != null) { Destroy(player.gameObject); }
            player = Jape.Game.CloneGameObject(Database.LoadPrefab("Player")).GetComponent<Player>();
            DontDestroyOnLoad(player);
            Instance.Player = player;
        }

        public static void Spawn()
        {
            string id = null;
            string scene = Jape.Game.ActiveScene().path;

            if (SaveManager.PullStatus("Spawn", out Status statusSpawn))
            {
                id = (string)statusSpawn.Read("Id");
                scene = (string)statusSpawn.Read("Scene");
            }

            Setup(() =>
            {
                CreatePlayer();
                MoveToSpawn(id);
            });
            Jape.Game.ChangeScene(scene);
        }

        public static void MoveCameraToPlayer()
        {
            Player player = Instance.Player;

            if (player == null) { return; }
            if (Jape.Game.DefaultCamera == null) { return; }

            Jape.Game.DefaultCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Jape.Game.DefaultCamera.transform.position.z);
        }

        public static void MoveToSpawn(string id)
        {
            Player player = Instance.Player;

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
                spawn = spawns.FirstOrDefault(s => s.gameObject.Identifier() == id);
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

        public class SpeedTimer
        {
            private readonly Timer timer;

            private float current;
            private float count;
            private float pause;

            public SpeedTimer()
            {
                timer = Jape.Timer.CreateGlobal().ChangeMode(Jape.Timer.Mode.Loop);
            }

            public void Start(float time = 0)
            {
                count = time;
                current = Time.RealtimeCount();

                if (!timer.IsProcessing())
                {
                    timer.Set(1, Time.Counter.Frames)
                         .IterationAction(Update)
                         .Start();
                }
            }

            public float Stop()
            {
                if (timer.IsProcessing())
                {
                    timer.Stop();
                }

                return count;
            }

            public void Pause()
            {
                if (pause > 0)
                {
                     this.Log().Diagnostic("Already paused");
                     return;
                }

                pause = Stop();
            }

            public void Resume()
            {
                if (pause <= 0)
                {
                    this.Log().Diagnostic("Not paused");
                    return;
                }

                Start(pause);
                pause = 0;
            }

            private void Update()
            {
                count += Time.RealtimeCount() - current;
                current = Time.RealtimeCount();
                Instance.UI.SetTime(count);

                Status status = new Status
                {
                    Key = "Time"
                };

                status.Write("Current", count);

                Status.Save(status);
            }
        }
    }
}