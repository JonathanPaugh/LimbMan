using UnityEngine;

namespace Jape
{
	public abstract class GameShell : Shell
    {
        protected override void Subscribe()
        {
            if (!EngineManager.IsQuitting())
            {
                EngineManager.Instance.OnGameInit += GameInit;
                EngineManager.Instance.OnGameSave += OnGameSave;
                EngineManager.Instance.OnGameLoad += OnGameLoad;
                EngineManager.Instance.OnSceneChange += OnSceneChange;
                EngineManager.Instance.OnSceneDestroy += OnSceneDestroy;
                EngineManager.Instance.OnSceneSetup += OnSceneSetup;
                EngineManager.Instance.OnSceneCreate += OnSceneCreate;
                EngineManager.Instance.OnSceneLoad += OnSceneLoad;
            }
        }

        protected override void Unsubscribe()
        {
            if (!EngineManager.IsQuitting())
            {
                EngineManager.Instance.OnGameInit -= GameInit;
                EngineManager.Instance.OnGameSave -= OnGameSave;
                EngineManager.Instance.OnGameLoad -= OnGameLoad;
                EngineManager.Instance.OnSceneChange -= OnSceneChange;
                EngineManager.Instance.OnSceneDestroy -= OnSceneDestroy;
                EngineManager.Instance.OnSceneSetup -= OnSceneSetup;
                EngineManager.Instance.OnSceneCreate -= OnSceneCreate;
                EngineManager.Instance.OnSceneLoad -= OnSceneLoad;
            }
        }

        private void GameInit()
        {
            if (!Application.isEditor) { OnBuildInit(); }
            OnGameInit();
        }
        
        protected virtual void OnBuildInit() {}
        protected virtual void OnGameInit() {}

        protected virtual void OnGameSave() {}
        protected virtual void OnGameLoad() {}

        protected virtual void OnSceneChange(Map map) {}
        protected virtual void OnSceneDestroy(Map map) {}
        protected virtual void OnSceneSetup(Map map) {}
        protected virtual void OnSceneCreate(Map map) {}
        protected virtual void OnSceneLoad(Map map) {}
    }
}