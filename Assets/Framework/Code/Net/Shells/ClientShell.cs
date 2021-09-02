using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public abstract class ClientShell : GameShell
    {
        protected override void Subscribe()
        {
            base.Subscribe();
            if (!NetManager.IsQuitting())
            {
                NetManager.Instance.OnStartClient += OnStart;
                NetManager.Instance.OnStartClient += OnStop;
                NetManager.Instance.OnConnectClient += OnConnect;
                NetManager.Instance.OnDisconnectClient += OnDisconnect;
                NetManager.Instance.OnPlayerConnectClient += OnPlayerConnect;
                NetManager.Instance.OnPlayerDisconnectClient += OnPlayerDisconnect;
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            if (!NetManager.IsQuitting())
            {
                NetManager.Instance.OnStartClient -= OnStart;
                NetManager.Instance.OnStartClient -= OnStop;
                NetManager.Instance.OnConnectClient -= OnConnect;
                NetManager.Instance.OnDisconnectClient -= OnDisconnect;
                NetManager.Instance.OnPlayerConnectClient -= OnPlayerConnect;
                NetManager.Instance.OnPlayerDisconnectClient -= OnPlayerDisconnect;
            }
        }

        protected sealed override void OnBuildInit() => base.OnGameInit();
        protected sealed override void OnGameInit() => base.OnGameInit();
        protected sealed override void OnGameLoad() => base.OnGameLoad();

        protected virtual void OnStart() {}
        protected virtual void OnStop() {}

        protected virtual void OnConnect(int client) {}
        protected virtual void OnDisconnect(int client) {}

        protected virtual void OnPlayerConnect(int client) {}
        protected virtual void OnPlayerDisconnect(int client) {}
        
    }
}