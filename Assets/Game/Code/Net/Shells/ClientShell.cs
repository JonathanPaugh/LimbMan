using UnityEngine;

namespace GameNet
{
	public class ClientShell : JapeNet.ClientShell
    {
        protected override void OnSceneChange(Map map) {}
        protected override void OnSceneDestroy(Map map) {}
        protected override void OnSceneSetup(Map map) {}
        protected override void OnSceneCreate(Map map) {}
        protected override void OnSceneLoad(Map map) {}

        protected override void OnStart() {}
        protected override void OnConnect(int client) {}
        protected override void OnPlayerConnect(int client) {}
        protected override void OnPlayerDisconnect(int client) {}
        protected override void OnDisconnect(int client) {}
    }
}