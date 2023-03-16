using System;
using System.Collections.Generic;
using System.Linq;
using Jape;

namespace JapeNet
{
    public abstract partial class NetElement : Element
    {
        protected const int DefaultStreamRate = 1;

        protected enum Communication
        {
            None,
            Remote,
            All,
        }

        protected NetMode Mode => NetManager.GetMode();
        protected virtual Communication CommuncationMode => Communication.Remote;
        
        protected virtual Type PairType => GetType();

        private Key cachedPairKey;
        public virtual Key PairKey => cachedPairKey ??= GeneratePairKey();

        protected virtual Key GeneratePairKey() => new(
            PairType, 
            gameObject.Identifier(), 
            gameObject.HasId() ? Key.IdentifierEncoding.Hex : Key.IdentifierEncoding.Ascii
        );
        
        protected virtual int ClientStreamRate => DefaultStreamRate;
        protected virtual int ServerStreamRate => DefaultStreamRate;

        private NetStream clientStream;
        private NetStream serverStream;

        private readonly Sync clientSync = new();
        private readonly Sync serverSync = new();

        protected bool HasServerAccess => !Mode.IsClientOnly;
        protected bool HasClientAccess
        {
            get
            {
                if (!Mode.IsOnline) { return true; }
                if (Mode.IsServerOnly) { return false; }
                return IsOwner(Client.Client.Id);
            }
        }

        internal override void Awake()
        {
            base.Awake();

            if (Game.IsRunning)
            {
                NetManager.Instance.runtimeNetElements.Add(this);

                clientStream = new NetStream(ClientStreamRate);
                serverStream = new NetStream(ServerStreamRate);
            }
        }

        internal override void OnDestroy()
        {
            base.OnDestroy();

            if (Game.IsRunning)
            {
                if (NetManager.Instance != null) { NetManager.Instance.runtimeNetElements.Remove(this); }
            }
        }

        internal override void Update()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsClient || !Mode.IsOnline)
                {
                    ReadStream(NetSide.Clientside);
                    ReadSync(NetSide.Clientside);
                }
            }

            base.Update();

            if (Game.IsRunning) 
            {
                if (Mode.IsClient || !Mode.IsOnline)
                {
                    WriteStream(NetSide.Clientside);
                    SendStreamData(NetSide.Clientside);

                    WriteSync(NetSide.Clientside);
                    SendSyncData(NetSide.Clientside);
                }
            }
        }

        internal override void FixedUpdate()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsServer || !Mode.IsOnline)
                {
                    ReadStream(NetSide.Serverside);
                    ReadSync(NetSide.Serverside);
                }
            }

            base.FixedUpdate();

            if (Game.IsRunning)
            {
                if (Mode.IsServer || !Mode.IsOnline)
                {
                    WriteStream(NetSide.Serverside);
                    SendStreamData(NetSide.Serverside);

                    WriteSync(NetSide.Serverside);
                    SendSyncData(NetSide.Serverside);
                }
            }
        }

        protected internal bool IsOwner(int player)
        {
            return gameObject.Player() == 0 || gameObject.Player() == player;
        }

        internal bool CanReceiveData(NetSide? side)
        {
            switch (CommuncationMode)
            {
                case Communication.None: return false;
                case Communication.Remote:
                {
                    switch (side)
                    {
                        case NetSide.Clientside: return Mode.IsClientOnly;
                        case NetSide.Serverside: return Client.Client.IsRemote(gameObject.Player());
                        default: return false;
                    }
                }
                default: return true;
            }
        }

        internal void SendData(NetSide side, Action sendClientData, Action<int> sendServerData, Action<NetElement> sendOfflineData)
        {
            if (CommuncationMode == Communication.None) { return; }

            bool CanSendClientData() => Client.Client.Connected && IsOwner(Client.Client.Id);

            Mode.DedicatedBranch
            (
                delegate
                {
                    switch (side)
                    {
                        case NetSide.Clientside: default:
                        {
                            if (!CanSendClientData()) { return; }
                            sendClientData?.Invoke();
                            break;
                        }
                        case NetSide.Serverside:
                        {
                            sendServerData?.Invoke(default);
                            break;
                        }
                    }
                },
                delegate
                {
                    switch (side)
                    {
                        case NetSide.Clientside: default:
                        {
                            if (!CanSendClientData()) { return; }
                            if (CommuncationMode == Communication.Remote) { return; }
                            sendClientData?.Invoke();
                            break;
                        }
                        case NetSide.Serverside:
                        {
                            sendServerData?.Invoke(
                                CommuncationMode == Communication.Remote
                                ? Client.Client.Id
                                : default
                            );
                            break;
                        }
                    }
                },
                delegate
                {
                    if (CommuncationMode == Communication.Remote) { return; }
                    NetManager.AccessOfflineElement(PairKey.Encode(), e => sendOfflineData?.Invoke(e));
                }
            );
        }

        protected void SendField(NetSide side, string name, object value)
        {
            SendData(
                side,
                () => Client.Client.Send.Field(PairKey, name, value),
                p => Server.Server.Send.Field(PairKey, name, value, p),
                e => e.ReceiveField(name, value)
            );
        }

        internal virtual void ReceiveField(string name, object value)
        {
            Member member = new(this, name);
            if (!Attribute.IsDefined(member.Target, typeof(ExposeAttribute), true)) { return; }
            member.Set(value);
        }

        protected void SendCall(NetSide side, string name, params object[] args)
        {
            SendData(
                side,
                () => Client.Client.Send.Call(PairKey, name, args),
                p => Server.Server.Send.Call(PairKey, name, args, p),
                e => e.ReceiveCall(name, args)
            );
        }

        internal virtual void ReceiveCall(string name, object[] args)
        {
            Member member = new(this, name);
            if (!Attribute.IsDefined(member.Target, typeof(CallAttribute), true)) { return; }
            member.Get(args);
        }

        internal void SendStreamData(NetSide side)
        {
            object[] data;

            switch (side)
            {
                case NetSide.Clientside: default:
                {
                    if (!clientStream.HasWriteData()) { return; }
                    data = clientStream.PullData();
                    break;
                }
                case NetSide.Serverside:
                {
                    if (!serverStream.HasWriteData()) { return; }
                    data = serverStream.PullData();
                    break;
                }
            }

            SendData(
                side,
                () => Client.Client.Send.Stream(PairKey, data),
                p => Server.Server.Send.Stream(PairKey, data, p),
                e =>
                {
                    switch (side)
                    {
                        case NetSide.Clientside: default:
                        {
                            e.clientStream.PushData(data);
                            break;
                        }
                        case NetSide.Serverside:
                        {
                            e.serverStream.PushData(data);
                            break;
                        }
                    }
                }
            );
        }

        internal virtual void ReceiveStreamData(NetSide side, object[] data)
        {
            switch (side)
            {
                case NetSide.Clientside: default: serverStream.PushData(data); break;
                case NetSide.Serverside: clientStream.PushData(data); break;
            }
        }

        internal void SendSyncData(NetSide side)
        {
            Dictionary<string, object> data;

            switch (side)
            {
                case NetSide.Clientside: default:
                {
                    if (!clientSync.CanSend()) { return; }
                    data = clientSync.Pull();
                    break;
                }
                case NetSide.Serverside:
                {
                    if (!serverSync.CanSend()) { return; }
                    data = serverSync.Pull();
                    break;
                }
            }

            SendData(
                side,
                () => Client.Client.Send.Sync(PairKey, data),
                p => Server.Server.Send.Sync(PairKey, data, p),
                e => e.ReceiveSyncData(side.Opposite(), data)
            );
        }

        internal virtual void ReceiveSyncData(NetSide side, Dictionary<string, object> data)
        {
            switch (side)
            {
                case NetSide.Clientside: default: serverSync.ReadData = data; break;
                case NetSide.Serverside: clientSync.ReadData = data; break;
            }
        }

        /// <summary>
        /// Streams data to server every frame
        /// </summary>
        protected virtual void SendStream(NetStream.IClientWriter stream) {}

        /// <summary>
        /// Streams data to client every tick
        /// </summary>
        protected virtual void SendStream(NetStream.IServerWriter stream) {}

        /// <summary>
        /// Streams data from server every frame
        /// </summary>
        protected virtual void ReceiveStream(NetStream.IClientReader stream) {}

        /// <summary>
        /// Streams data from client every tick
        /// </summary>
        protected virtual void ReceiveStream(NetStream.IServerReader stream) {}

        internal void ReadStream(NetSide side)
        {
            NetStream stream = side == NetSide.Clientside 
                ? serverStream 
                : clientStream;

            if (!stream.HasReadData()) { return; }

            switch (side)
            {
                case NetSide.Clientside: default:
                {
                    stream.ReadNext(() =>
                    {
                        ReceiveStream((NetStream.IClientReader)stream.reader);
                    });
                    break;
                }
                case NetSide.Serverside:
                {
                    stream.ReadNext(() =>
                    {
                        ReceiveStream((NetStream.IServerReader)stream.reader);
                    });
                    break;
                }
            }

            stream.ClearReadData();
        }

        internal void WriteStream(NetSide side)
        {
            NetStream stream = side == NetSide.Clientside 
                ? clientStream
                : serverStream;

            stream.ClearWriteData();

            switch (side)
            {
                case NetSide.Clientside: default:
                {
                    stream.WriteNext(() =>
                    {
                        SendStream((NetStream.IClientWriter)stream.writer);
                    });
                    break;
                }
                case NetSide.Serverside:
                {
                    stream.WriteNext(() =>
                    {
                        SendStream((NetStream.IServerWriter)stream.writer);
                    });
                    break;
                }
            }
        }

        protected virtual void ReadSync(NetSide side)
        {
            Sync sync = side == NetSide.Clientside 
                ? serverSync
                : clientSync;

            foreach (var (field, attribute) in GetAttributeMembers<ReceiveSyncAttribute>(true))
            {
                if (side != attribute.Side) { continue; }

                string key = attribute.Key ?? field.Name;

                if (sync.Read(key, out object value))
                {
                    field.SetValue(this, value);
                }
            }
        }

        protected virtual void WriteSync(NetSide side)
        {
            Sync sync = side == NetSide.Clientside 
                ? clientSync 
                : serverSync;

            foreach (var (field, attribute) in GetAttributeMembers<SendSyncAttribute>(true))
            {
                if (side != attribute.Side) { continue; }

                string key = attribute.Key ?? field.Name;
                object value = field.GetValue(this);

                sync.Write(key, value);
            }
        }

        internal static NetElement FindByKey(byte[] key)
        {
            return NetManager.Instance.runtimeNetElements.FirstOrDefault(element => element.Key.Compare(key));
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        protected class SyncAttribute : Attribute
        {
            internal NetSide Side { get; }
            internal string Key { get; }

            internal SyncAttribute(NetSide side, string key = null)
            {
                Side = side;
                Key = key;
            }
        }

        protected class ReceiveSyncAttribute : SyncAttribute
        {
            public ReceiveSyncAttribute(NetSide side, string key = null) : base(side, key) {}
        }

        protected class SendSyncAttribute : SyncAttribute
        {
            public SendSyncAttribute(NetSide side, string key = null) : base(side, key) {}
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        protected class ExposeAttribute : Attribute {}

        [AttributeUsage(AttributeTargets.Method)]
        protected class CallAttribute : Attribute {}
    }
}