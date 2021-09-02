using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public abstract class ServerShell : GameShell
    {
        protected override void Subscribe()
        {
            base.Subscribe();
            if (!NetManager.IsQuitting())
            {
                NetManager.Instance.OnStartServer += OnStart;
                NetManager.Instance.OnStartServer += OnStop;
                NetManager.Instance.OnPlayerConnectServerFirst += OnConnectFirst;
                NetManager.Instance.OnPlayerConnectServer += OnConnect;
                NetManager.Instance.OnPlayerSceneChangeServer += OnSceneChange;
                NetManager.Instance.OnPlayerDisconnectServer += OnDisconnect;
                NetManager.Instance.OnPlayerDisconnectServerLast += OnDisconnectLast;
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            if (!NetManager.IsQuitting())
            {
                NetManager.Instance.OnStartServer -= OnStart;
                NetManager.Instance.OnStartServer -= OnStop;
                NetManager.Instance.OnPlayerConnectServerFirst -= OnConnectFirst;
                NetManager.Instance.OnPlayerConnectServer -= OnConnect;
                NetManager.Instance.OnPlayerSceneChangeServer -= OnSceneChange;
                NetManager.Instance.OnPlayerDisconnectServer -= OnDisconnect;
                NetManager.Instance.OnPlayerDisconnectServerLast -= OnDisconnectLast;
            }
        }

        protected sealed override void OnBuildInit() => base.OnGameInit();
        protected sealed override void OnGameInit() => base.OnGameInit();
        protected sealed override void OnGameLoad() => base.OnGameLoad();

        protected virtual void OnStart() {}
        protected virtual void OnStop() {}

        protected virtual void OnConnectFirst() {}
        protected virtual void OnConnect(int client) {}
        protected virtual void OnSceneChange(int client) {}
        protected virtual void OnDisconnect(int client) {}
        protected virtual void OnDisconnectLast() {}
    }
}