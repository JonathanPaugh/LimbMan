using System;
using System.Collections.Generic;
using JapeNet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameNet
{
	public class NetShell : JapeNet.NetShell
    {
        protected override void OnSceneChange(Map map) {}
        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}
        protected override void OnSceneCreate(Map map) {}
        protected override void OnSceneLoad(Map map) {}

        public override Dictionary<string, Action<object[]>> Delegates => new Dictionary<string, Action<object[]>>();
    }
}