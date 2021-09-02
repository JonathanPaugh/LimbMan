using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
    public sealed class EngineManager : Manager<EngineManager>
    {
        [NonSerialized] public Action OnGameInit = delegate {};
        [NonSerialized] public Action OnGameLoad = delegate {};

        [NonSerialized] public Action<Map> OnSceneChange = delegate {};
        [NonSerialized] public Action<Map> OnSceneDestroy = delegate {};
        [NonSerialized] public Action<Map> OnSceneSetup = delegate {};
        [NonSerialized] public Action<Map> OnSceneCreate = delegate {};
        [NonSerialized] public Action<Map> OnSceneLoad = delegate {};

        [NonSerialized] internal List<Mono> runtimeMono = new List<Mono>();
        [NonSerialized] internal List<Element> runtimeElements = new List<Element>();
        [NonSerialized] internal List<Scriptable> runtimeScriptable = new List<Scriptable>();

        [NonSerialized] internal List<Shell> shells = new List<Shell>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnInit()
        {
            Application.runInBackground = true;
            Random.InitState();
            Global.InitAll();
            Serializer.Init();
            InitTime();
            InitManagers();
            Startup();
            AddShell(typeof(GameShell));
            Instance.OnGameInit.Invoke();
        }

        protected override void PreDestroy()
        {
            for (int i = runtimeMono.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(runtimeMono[i]);
            }
        }

        protected override void Init() { statsJob = Job.CreateGlobal().Set(StatRoutine()).Start(); }
        protected override void First() { Timer.Delay(1, Time.Counter.Frames, () => Instance.OnSceneLoad.Invoke(new Map(Game.ActiveScene().path))); }
        protected override void Tick() { Time.tickCount++; }
        
        private static void Startup()
        {
            foreach (GameObject gameObject in Game.Settings<CacheSettings>().startup) { DontDestroyOnLoad(Game.CloneGameObject(gameObject)); }
        }

        private static void InitTime()
        {
            Time.Settings.Apply();
        }

        private static void InitManagers()
        {
            if (Game.IsWeb)
            {
                if (!WebManager.HasInstance())
                {
                    WebManager.CreateInstance();
                }
            }

            List<Type> managers = typeof(Manager<>).GetSubclass().Where(t => t != typeof(EngineManager)).ToList();

            foreach (Type manager in managers.Where(m => Member.Static(typeof(Manager<>).MakeGenericType(m), "instance").Get() == null))
            {
                Member.LogOff();
                Member member = Member.Static(manager.Assembly, manager.Name, nameof(InitOnLoad));
                Member.LogOn();
                if (member.Target != null && (bool)member.Get())
                {
                    Member.Static(typeof(Manager<>).MakeGenericType(manager), nameof(CreateInstance)).Get();
                }
            }
        }

        public static void AddShell(Type parentShell)
        {
            if (IsQuitting()) { return; }

            if (!parentShell.IsBaseOrSubclassOf(typeof(Shell)))
            {
                Log.Write($"Unable to add shell type: {parentShell}");
            }

            foreach (Type type in parentShell.GetSubclass(true).Where(t => t.BaseType == parentShell))
            {
                if (Instance.gameObject.HasComponent(type, false))
                {
                    Log.Warning($"Unable to add multiple shell type: {type}");
                    return;
                }

                Instance.gameObject.AddComponent(type);
            }
        }

        public static void RemoveShell(Type parentShell)
        {
            if (IsQuitting()) { return; }

            if (!parentShell.IsBaseOrSubclassOf(typeof(Shell)))
            {
                Log.Write($"Unable to remove shell type: {parentShell}");
            }

            foreach (Type type in parentShell.GetSubclass(true).Where(t => t.BaseType == parentShell))
            {
                Component component = Instance.gameObject.GetComponent(type);
                if (component != null)
                {
                    Destroy(component);
                }
            }
        }

        internal static void SaveGame()
        {
            SaveManager.Save();
        }

        internal static void LoadGame(Action<bool> onLoad)
        {
            SaveManager.Load(b =>
            {
                onLoad?.Invoke(b);
                if (b) { Instance.OnGameLoad.Invoke(); }
            });
        }

        private bool changingScene;
        internal static void ChangeScene(string scenePath, string loadingScreenPath, Action loaded)
        {
            if (Instance == null) { return; }
            if (Instance.changingScene) { Instance.Log().Response("Scene is already changing"); return; }

            SceneSettings settings = Game.Settings<SceneSettings>();

            string @base = settings.baseMap.ScenePath;
            Scene active = Game.ActiveScene();
            Timer timer = Timer.CreateGlobal().Set(settings.sceneFade);
            Job.CreateGlobal().Set(Change()).Start();

            IEnumerable Change()
            {
                Instance.changingScene = true;

                Instance.OnSceneChange.Invoke(new Map(scenePath));

                yield return LoadBase();

                yield return DestroyActive();

                Instance.OnSceneDestroy.Invoke(new Map(active.path));

                yield return UnloadActive();

                Instance.OnSceneSetup.Invoke(new Map(scenePath));
                
                yield return LoadScene();

                Instance.OnSceneCreate.Invoke(new Map(scenePath));

                yield return UnloadBase();

                Instance.changingScene = false;

                loaded?.Invoke();

                Instance.OnSceneLoad.Invoke(new Map(scenePath));

                IEnumerator LoadBase()
                {
                    AsyncOperation loadBase = SceneManager.LoadSceneAsync(@base, LoadSceneMode.Additive);
                    if (Game.DefaultCamera != null)
                    {
                        timer.IntervalAction(t => Game.DefaultCamera.overlay.SetColor(Fade(t.Progress()))).Start();
                        yield return timer.WaitIdle();
                    }
                    yield return Wait.Until(() => loadBase.isDone);
                }

                IEnumerator DestroyActive()
                {
                    ModuleManager.Instance.DestroyAll();

                    yield return Wait.Until(() => ModuleManager.Instance.GetModules().All(m => !m.IsProcessing()));
                    yield return Wait.Frame();

                    GameObject[] gameObjects = active.GetRootGameObjects();
                    for (int i = gameObjects.Length - 1; i >= 0; i--) { Destroy(gameObjects[i]); }

                    yield return Wait.Frame();
                    yield return Wait.Frame();
                }

                IEnumerator UnloadActive()
                {
                    AsyncOperation unloadActive = SceneManager.UnloadSceneAsync(active);

                    yield return Wait.Until(() => unloadActive.isDone);

                    if (!string.IsNullOrEmpty(loadingScreenPath))
                    {
                        AsyncOperation loadLoadingScreen = SceneManager.LoadSceneAsync(loadingScreenPath, LoadSceneMode.Additive);
                        yield return Wait.Until(() => loadLoadingScreen.isDone);

                        if (Game.DefaultCamera != null)
                        {
                            timer.IntervalAction(t => Game.DefaultCamera.overlay.SetColor(Fade(t.Inverse()))).Start();
                            yield return timer.WaitIdle();
                        }
                    }
                }

                IEnumerator LoadScene()
                {
                    AsyncOperation loadScene = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                    yield return Wait.Until(() => loadScene.isDone);

                    if (!string.IsNullOrEmpty(loadingScreenPath))
                    {
                        if (Game.DefaultCamera != null)
                        {
                            timer.IntervalAction(t => Game.DefaultCamera.overlay.SetColor(Fade(t.Progress()))).Start();
                            yield return timer.WaitIdle();
                        }

                        AsyncOperation unloadLoadingScreen = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(loadingScreenPath));
                        yield return Wait.Until(() => unloadLoadingScreen.isDone);
                    }
                }

                IEnumerator UnloadBase()
                {
                    AsyncOperation unloadBase = SceneManager.UnloadSceneAsync(@base);

                    if (Game.DefaultCamera != null)
                    {
                        timer.IntervalAction(t => Game.DefaultCamera.overlay.SetColor(Fade(t.Inverse()))).Start();
                        yield return timer.WaitIdle();
                    }

                    yield return Wait.Until(() => unloadBase.isDone);
                }

                Color Fade(float progress) { return Color.Lerp(Color.clear, Color.black, progress); }
            }
        }

        [NonSerialized] 
        public Vector2 stats;
        private Job statsJob;
        private IEnumerable StatRoutine()
        {
            Vector2 previous = new Vector2();

            while (Application.isPlaying) 
            {
                Vector2 current = new Vector2(Time.FrameCount(), Time.TickCount());
                yield return Wait.Realtime(1);
                stats = current - previous;
                MetricManager.Set("Frames", stats.x.ToString(CultureInfo.InvariantCulture));
                MetricManager.Set("Ticks", stats.y.ToString(CultureInfo.InvariantCulture));
                previous = current;
            }
        }
    }
}