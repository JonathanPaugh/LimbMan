using System;
using System.Linq;
using Jape;
using UnityEngine;

namespace Game
{
	public class GameShell : Jape.GameShell
    {
        protected override void OnBuildInit() {}
        protected override void OnGameInit()
        {
            GameManager.Init();
        }

        protected override void OnGameSave() {}
        protected override void OnGameLoad() {}

        protected override void OnSceneChange(Map map)
        {
            GameManager.Instance.Timer.Log().ToggleDiagnostics();
            GameManager.Instance.Timer.Pause();
            GameManager.Instance.Timer.Log().ToggleDiagnostics();
        }

        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}

        protected override void OnSceneCreate(Map map)
        {
            GameManager.MoveCameraToPlayer();

            GameManager.Instance.Timer.Log().ToggleDiagnostics();
            GameManager.Instance.Timer.Resume();
            GameManager.Instance.Timer.Log().ToggleDiagnostics();
        }

        protected override void OnSceneLoad(Map map) {} 
    }
}