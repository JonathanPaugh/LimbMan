using System;

namespace JapeNet
{
	public readonly struct NetMode
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

        private static readonly NetMode offline = new(Mode.Offline);
        private static readonly NetMode client = new(Mode.Client);
        private static readonly NetMode server = new(Mode.Server);
        private static readonly NetMode host = new(Mode.Host);

        private readonly Mode mode;

        private NetMode(Mode mode)
        {
            this.mode = mode;
        }

        public void SideBranch(Action clientAction, Action serverAction, Action offlineAction = null)
        {
            if (IsOnline)
            {
                if (IsClient)
                {
                    clientAction?.Invoke();
                } 
                if (IsServer)
                {
                    serverAction?.Invoke();
                }
            } 
            else
            {
                offlineAction?.Invoke();
            }
        }

        public void DedicatedBranch(Action dedicatedAction, Action hostAction, Action offlineAction = null)
        {
            if (IsOnline)
            {
                if (IsP2P)
                {
                    hostAction?.Invoke();
                }
                else
                {
                    dedicatedAction?.Invoke();
                }
            } 
            else
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