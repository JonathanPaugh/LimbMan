using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class NetElement : Element
    {
        private const int OfflinePlayer = 1;

        protected enum Communication
        {
            None,
            Remote,
            All,
        }

        internal virtual NetMode NetSide => Mode;
        protected virtual Communication CommuncationMode => Communication.Remote;

        protected NetMode Mode => NetManager.GetMode();
        
        protected virtual Type PairType => GetType();
        protected virtual Type[] PairComponents => null;

        private Key cachedPairKey;
        public virtual Key PairKey => cachedPairKey ??= GeneratePairKey();

        protected virtual Key GeneratePairKey() => new Key(
            PairType, 
            gameObject.Identifier(), 
            gameObject.HasId() ? Key.IdentifierEncoding.Hex : Key.IdentifierEncoding.ASCII
        );
        
        protected NetStream clientToServerStream = new NetStream();
        protected NetStream serverToClientStream = new NetStream();

        private Dictionary<string, SyncData> syncData = new Dictionary<string, SyncData>();

        internal bool CanReceiveCommunication(NetMode mode)
        {
            switch (CommuncationMode)
            {
                case Communication.None: return false;
                case Communication.Remote:
                {
                    if (mode.IsClient)
                    {
                        return Mode.IsClientOnly;
                    }

                    if (mode.IsServer)
                    {
                        return Client.Client.IsRemote(gameObject.Player());
                    }

                    throw new Exception("Unable to resolve communication mode");
                }
                default: return true;
            }
        }

        internal bool CanAccess(int player)
        {
            return gameObject.Player() == 0 || gameObject.Player() == player;
        }

        protected bool ClientAccess()
        {
            if (Mode.IsServerOnly) { return false; }
            return Mode.IsOnline ? CanAccess(Client.Client.Id) : CanAccess(OfflinePlayer);
        }
        
        protected bool ServerAccess()
        {
            return !Mode.IsClientOnly;
        }

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsOnline)
                {
                    if (!Mode.HasFlag(NetSide))
                    {
                        DestroyImmediate(this);
                        DestroyPairComponents();
                        return;
                    }
                }
            } 

            base.Awake();

            if (Game.IsRunning)
            {
                NetManager.Instance.runtimeNetElements.Add(this);
            }
        }

        internal override void OnDestroy()
        {
            base.OnDestroy(); // First //

            if (Game.IsRunning)
            {
                if (NetManager.Instance != null) { NetManager.Instance.runtimeNetElements.Remove(this); }
            }
        }

        internal override void Update()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsClient)
                {
                    ReadStream(NetMode.Client);
                    ReadSync();
                }

                if (!Mode.IsOnline)
                {
                    ReadStream(NetMode.Client);
                    ReadSync();
                }
            }

            base.Update();

            if (Game.IsRunning) 
            {
                if (Mode.IsClient)
                {
                    WriteStream(NetMode.Client);
                    SendStreamData(NetMode.Client);
                }

                if (!Mode.IsOnline)
                {
                    WriteStream(NetMode.Client);

                    if (clientToServerStream.CanSendData())
                    {
                        NetManager.Client.AccessElement(PairKey.Encode(), e =>
                        {
                            e.PushClientStream(PullClientStream());
                        });
                    }
                }
            }
        }

        internal override void FixedUpdate()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsServer)
                {
                    ReadStream(NetMode.Server);
                }

                if (!Mode.IsOnline)
                {
                    ReadStream(NetMode.Server);
                }
            }

            base.FixedUpdate();

            if (Game.IsRunning)
            {
                if (Mode.IsServer)
                {
                    WriteStream(NetMode.Server);
                    SendStreamData(NetMode.Server);

                    WriteSync();
                    SendSyncData(NetMode.Server);
                }

                if (!Mode.IsOnline)
                {
                    WriteStream(NetMode.Server);
                        
                    if (serverToClientStream.CanSendData())
                    {
                        NetManager.Client.AccessElement(PairKey.Encode(), e =>
                        {
                            e.PushServerStream(PullServerStream());
                        });
                    }

                    WriteSync();
                    SendSyncData(NetMode.Server);
                }
            }
        }

        protected void SendField(string name, object value)
        {
            Mode.Branch
            (
                delegate
                {
                    if (!Client.Client.Connected) { return; }
                    if (!CanAccess(Client.Client.Id)) { return; }
                    Client.Client.Send.Field(PairKey, name, value);
                },
                delegate
                {
                    Server.Server.Send.Field(PairKey, name, value);
                },
                delegate
                {
                    NetManager.Client.AccessElement(PairKey.Encode(), e =>
                    {
                        Member.Set(e, name, value);
                    });
                }
            );
        }

        protected void SendCall(string name, params object[] args)
        {
            Mode.Branch
            (
                delegate
                {
                    if (!Client.Client.Connected) { return; }
                    if (!CanAccess(Client.Client.Id)) { return; }
                    Client.Client.Send.Call(PairKey, name, args);
                },
                delegate
                {
                    Server.Server.Send.Call(PairKey, name, args);
                },
                delegate
                {
                    NetManager.Client.AccessElement(PairKey.Encode(), e =>
                    {
                        Member.Get(e, name, null, args);
                    });
                }
            );
        }

        internal void ReceiveCall(string name, object[] args)
        {
            Member member = new Member(this, name);
            if (!Attribute.IsDefined(member.Target, typeof(CallAttribute))) { return; }
            member.Get(args);
        }

        /// <summary>
        /// Streams data to server every frame
        /// </summary>
        protected virtual void SendStream(NetStream.ClientWriter stream) {}

        /// <summary>
        /// Streams data to client every tick
        /// </summary>
        protected virtual void SendStream(NetStream.ServerWriter stream) {}

        /// <summary>
        /// Streams data from server every frame
        /// </summary>
        protected virtual void ReceiveStream(NetStream.ClientReader stream) {}

        /// <summary>
        /// Streams data from client every tick
        /// </summary>
        protected virtual void ReceiveStream(NetStream.ServerReader stream) {}

        internal virtual void PushClientStream(object[] data) { clientToServerStream.PushData(data); }
        internal virtual object[] PullClientStream() { return clientToServerStream.ToWriteDataArray(); }

        internal virtual void PushServerStream(object[] data) { serverToClientStream.PushData(data); }
        internal virtual object[] PullServerStream() { return serverToClientStream.ToWriteDataArray(); }

        internal void ReadStream(NetMode mode)
        {
            mode.Branch
            (
                delegate
                {
                    Read(serverToClientStream, stream =>
                    {
                        ReceiveStream((NetStream.ClientReader)stream.reader);
                    });
                },
                delegate
                {
                    Read(clientToServerStream, stream =>
                    {
                        ReceiveStream((NetStream.ServerReader)stream.reader);
                    });
                }
            );

            static void Read(NetStream stream, Action<NetStream> onRead)
            {
                if (stream.ToReadDataArray().Length <= 0) { return; }
                stream.StartReading();
                onRead(stream);
                stream.Stop();
                stream.ClearReadData();
            }
        }

        internal void WriteStream(NetMode mode)
        {
            mode.Branch
            (
                delegate
                {
                    Write(clientToServerStream, stream =>
                    {
                        SendStream((NetStream.ClientWriter)stream.writer);
                    });
                },
                delegate
                {
                    Write(serverToClientStream, stream =>
                    {
                        SendStream((NetStream.ServerWriter)stream.writer);
                    });
                }
            );

            static void Write(NetStream stream, Action<NetStream> onWrite)
            {
                stream.ClearWriteData();
                stream.StartWriting();
                onWrite(stream);
                stream.Stop();
            }
        }

        protected void SendStreamData(NetMode mode)
        {
            mode.Branch
            (
                delegate
                {
                    if (!Client.Client.Connected) { return; }
                    if (!CanAccess(Client.Client.Id)) { return; }
                    if (!clientToServerStream.CanSendData()) { return; }
                    Client.Client.Send.Stream(PairKey, PullClientStream()); 
                },
                delegate
                {
                    if (!serverToClientStream.CanSendData()) { return; }
                    Server.Server.Send.Stream(PairKey, PullServerStream()); 
                }
            );
        }

        internal void PushSyncData(Dictionary<string, object> data) { syncData = data.ToDictionary(d => d.Key, d => new SyncData(d.Value)); }
        internal Dictionary<string, object> PullSyncData() { return syncData.Where(d => d.Value.CanSend()).ToDictionary(d => d.Key, d => d.Value.Send()); }

        internal void SendSyncData(NetMode mode)
        {
            if (!syncData.Any()) { return; }

            Dictionary<string, object> data = PullSyncData();

            if (!data.Any()) { return; }

            mode.Branch
            (
                null,
                delegate
                {
                    Server.Server.Send.Sync(PairKey, data); 
                },
                delegate
                {
                    NetManager.Client.AccessElement(PairKey.Encode(), e =>
                    {
                        e.PushSyncData(data);
                    });
                }
            );
        }

        protected virtual void WriteSync()
        {
            foreach (FieldInfo field in GetSyncedMembers())
            {
                SyncAttribute attribute = field.GetCustomAttribute<SyncAttribute>();
                string key = attribute.Key ?? field.Name;
                object value = field.GetValue(this);
                if (syncData.ContainsKey(key)) { syncData[key].Set(value); } 
                else { syncData.Add(key, new SyncData(value)); }
            }
        }

        protected virtual void ReadSync()
        {
            foreach (FieldInfo field in GetSyncedMembers())
            {
                SyncAttribute attribute = field.GetCustomAttribute<SyncAttribute>();
                string key = attribute.Key ?? field.Name;
                if (syncData.ContainsKey(key))
                {
                    field.SetValue(this, syncData[key].Send());
                } 
            }
            syncData.Clear();
        }

        private IEnumerable<FieldInfo> GetSyncedMembers()
        {
            return GetType().GetFields(Member.Bindings).Where(f => Attribute.IsDefined(f, typeof(SyncAttribute)));
        }

        private void DestroyPairComponents()
        {
            if (PairComponents == null) { return; }
            foreach (Component component in PairComponents.Select(GetComponent).Where(c => c != null))
            {
                DestroyImmediate(component);
            }
        }

        public enum SyncMode
        {
            ServerToClient,
            ClientToServer,
        }

        public class SyncData
        {
            private object value;
            private bool sent;

            public SyncData(object value) { this.value = value; }
            public bool CanSend() { return !sent; }

            public void Set(object value)
            {
                if (this.value.Equals(value)) { return; }
                sent = false;
                this.value = value;
            }

            public object Send()
            {
                sent = true;
                return value;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class SyncAttribute : Attribute
        {
            internal string Key { get; }
            public SyncAttribute(string key = null) { Key = key; }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class CallAttribute : Attribute {}
    }
}