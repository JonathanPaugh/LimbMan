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
            Game.Timer.Log().ToggleDiagnostics();
            Game.Timer.Pause();
            Game.Timer.Log().ToggleDiagnostics();
        }

        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}

        protected override void OnSceneCreate(Map map)
        {
            GameManager.MoveCameraToPlayer();

            Game.Timer.Log().ToggleDiagnostics();
            Game.Timer.Resume();
            Game.Timer.Log().ToggleDiagnostics();
        }

        protected override void OnSceneLoad(Map map) {} 
    }
}