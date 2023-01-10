using System;
using System.Runtime.CompilerServices;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public struct NetMode
    {
        [Flags]
        private enum Mode
        {
            Offline = 0,
            Client = 1,
            Server = 2, // Dedicated
            Host = 3, // P2P
        }

        public bool IsOnline => mode != Mode.Offline;
        public bool IsP2P => mode == Mode.Host;
        public bool IsLocal => !IsOnline || IsP2P;

        public bool IsClientOnly => mode == Mode.Client;
        public bool IsServerOnly => mode == Mode.Server;

        public bool IsClient => mode.HasFlag(Mode.Client);
        public bool IsServer => mode.HasFlag(Mode.Server);

        public static NetMode Offline => offline;
        public static NetMode Client => client;
        public static NetMode Server => server;
        public static NetMode Host => host;

        private static readonly NetMode offline = new NetMode(Mode.Offline);
        private static readonly NetMode client = new NetMode(Mode.Client);
        private static readonly NetMode server = new NetMode(Mode.Server);
        private static readonly NetMode host = new NetMode(Mode.Host);

        private readonly Mode mode;

        private NetMode(Mode mode)
        {
            this.mode = mode;
        }

        public void Branch(Action clientAction, Action serverAction, Action offlineAction = null)
        {
            if (IsClient)
            {
                clientAction?.Invoke();
            } 
            if (IsServer)
            {
                serverAction?.Invoke();
            }
            if (!IsOnline)
            {
                offlineAction?.Invoke();
            }
        }

        public bool HasFlag(NetMode mode) => this.mode.HasFlag(mode.mode);

        public override bool Equals(object obj)
        {
            if (!(obj is NetMode netMode)) { return false; }
            return mode == netMode.mode;
        }

        public override int GetHashCode() { return mode.GetHashCode(); }

        public static bool operator ==(NetMode mode1, NetMode mode2) { return mode1.mode == mode2.mode; }
        public static bool operator !=(NetMode mode1, NetMode mode2) => !(mode1 == mode2);
    }
}