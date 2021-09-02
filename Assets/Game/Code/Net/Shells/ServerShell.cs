using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jape;
using JapeNet;
using UnityEngine;

namespace GameNet
{
	public class ServerShell : JapeNet.ServerShell
    {
        private List<GameObject> players = new List<GameObject>();

        protected override void OnSceneChange(Map map) {}
        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}
        protected override void OnSceneCreate(Map map) {}
        protected override void OnSceneLoad(Map map) {}

        protected override void OnStart() {}

        protected override void OnConnectFirst() {} 
        protected override void OnConnect(int client) {}

        protected override void OnDisconnect(int client) {}
        protected override void OnDisconnectLast() {} 

        protected override void OnSceneChange(int client) {}
    }
}