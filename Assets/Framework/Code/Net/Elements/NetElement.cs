using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace JapeNet
{
    public abstract class NetElement : Element
    {
        protected virtual NetManager.Mode Mode => NetManager.GetMode();
        
        protected virtual Type PairType => GetType();
        protected virtual Type[] PairComponents => null;

        public virtual string PairKey => !string.IsNullOrEmpty(gameObject.Id()) ? 
                                         $"{PairType.FullName}_{gameObject.Id()}" : 
                                         $"{PairType.FullName}_{gameObject.Alias()}";
        
        protected NetStream stream;

        protected virtual int ClientStreamRate => NetManager.Settings.clientStreamRate;
        protected virtual int ServerStreamRate => NetManager.Settings.serverStreamRate;

        private Dictionary<string, SyncData> syncData = new Dictionary<string, SyncData>();

        internal bool CanAccess(int player)
        {
            switch (NetManager.GetMode())
            {
                case NetManager.Mode.Offline:
                    return true;

                default:
                    if (gameObject.Player() == 0) { return true; }
                    return gameObject.Player() == player;  
            }
        }

        protected bool ClientAccess()
        {
            if (NetManager.GetMode() == NetManager.Mode.Server) { return false; }
            return CanAccess(Client.Client.server.id);
        }

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode() != NetManager.Mode.Offline)
                {
                    if (NetManager.GetMode() != Mode)
                    {
                        DestroyImmediate(this);
                        return;
                    }

                    DestroyPairComponents();
                }
            } 

            base.Awake();

            if (Game.IsRunning)
            {
                NetManager.Instance.runtimeNetElements.Add(this);
            }

            if (Game.IsRunning)
            {
                stream = new NetStream(ClientStreamRate, ServerStreamRate);
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
                if (NetManager.GetMode() == NetManager.Mode.Client)
                {
                    ReadStream(NetManager.Mode.Client);
                    ReadSync();
                }
            }

            base.Update();

            if (Game.IsRunning) 
            {
                if (NetManager.GetMode() == NetManager.Mode.Client)
                {
                    IncrementStream(NetManager.Mode.Client);
                    WriteStream(NetManager.Mode.Client);
                    SendStreamData();
                }
            }
        }

        internal override void FixedUpdate()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Server)
                {
                    ReadStream(NetManager.Mode.Server);
                }
            }

            base.FixedUpdate();

            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Server)
                {
                    IncrementStream(NetManager.Mode.Server);
                    WriteStream(NetManager.Mode.Server);
                    SendStreamData();

                    WriteSync();
                    SendSyncData();
                }
            }
        }

        protected void SendField(string name, object value)
        {
            switch (NetManager.GetMode())
            {
                case NetManager.Mode.Offline:
                    NetManager.Client.AccessElement(PairKey, e =>
                    {
                        Member.Set(e, name, value);
                    });
                    return;

                case NetManager.Mode.Client: 
                    if (Client.Client.server == null) { return; }
                    if (!Client.Client.server.connected) { return; }
                    if (!CanAccess(Client.Client.server.id)) { return; }
                    Client.Client.Send.Field(PairKey, name, value);
                    return;

                case NetManager.Mode.Server: 
                    Server.Server.Send.Field(PairKey, name, value);
                    return;
            }
        }

        protected void SendCall(string name, params object[] args)
        {
            switch (NetManager.GetMode())
            {
                case NetManager.Mode.Offline:
                    NetManager.Client.AccessElement(PairKey, e =>
                    {
                        Member.Get(e, name, null, args);
                    });
                    return;

                case NetManager.Mode.Client:
                    if (Client.Client.server == null) { return; }
                    if (!Client.Client.server.connected) { return; }
                    if (!CanAccess(Client.Client.server.id)) { return; }
                    Client.Client.Send.Call(PairKey, name, args);
                    return;

                case NetManager.Mode.Server: 
                    Server.Server.Send.Call(PairKey, name, args);
                    return;
            }
        }

        internal void ReceiveCall(string name, object[] args)
        {
            Member member = new Member(this, name);
            if (!Attribute.IsDefined(member.Target, typeof(CallAttribute))) { return; }
            member.Get(args);
        }

        protected void SendStreamData()
        {
            if (!stream.CanSendData()) { return; }
            switch (NetManager.GetMode())
            {
                case NetManager.Mode.Offline:
                    NetManager.Client.AccessElement(PairKey, e =>
                    {
                        e.PushStreamData(PullStreamData());
                    });
                    return;

                case NetManager.Mode.Client: 
                    if (Client.Client.server == null) { return; }
                    if (!Client.Client.server.connected) { return; }
                    if (!CanAccess(Client.Client.server.id)) { return; }
                    Client.Client.Send.Stream(PairKey, PullStreamData()); 
                    return;

                case NetManager.Mode.Server: 
                    Server.Server.Send.Stream(PairKey, PullStreamData()); 
                    return;
            }
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

        internal virtual void PushStreamData(object[] data) { stream.PushData(data); }
        internal virtual object[] PullStreamData() { return stream.ToWriteDataArray(); }

        internal void ReadStream(NetManager.Mode mode)
        {
            if (stream.ToReadDataArray().Length <= 0) { return; }

            stream.StartReading();

            switch (mode)
            {
                case NetManager.Mode.Client:
                    ReceiveStream((NetStream.ClientReader)stream.reader);
                    break;

                case NetManager.Mode.Server:
                    ReceiveStream((NetStream.ServerReader)stream.reader);
                    break;
            }
            
            stream.Stop();
            stream.ClearReadData();
        }

        internal void WriteStream(NetManager.Mode mode)
        {
            stream.ClearWriteData();
            stream.StartWriting();

            switch (mode)
            {
                case NetManager.Mode.Client:
                    stream.writer.StartClient();
                    SendStream((NetStream.ClientWriter)stream.writer);
                    break;

                case NetManager.Mode.Server:
                    stream.writer.StartServer();
                    SendStream((NetStream.ServerWriter)stream.writer);
                    break;
            }

            stream.writer.Stop();
            stream.Stop();
        }

        internal void IncrementStream(NetManager.Mode mode)
        {
            switch (mode)
            {
                case NetManager.Mode.Client:
                    stream.writer.IncrementClient();
                    break;

                case NetManager.Mode.Server:
                    stream.writer.IncrementServer();
                    break;
            }
        }

        internal void PushSyncData(Dictionary<string, object> data) { syncData = data.ToDictionary(d => d.Key, d => new SyncData(d.Value)); }
        internal Dictionary<string, object> PullSyncData() { return syncData.Where(d => d.Value.CanSend()).ToDictionary(d => d.Key, d => d.Value.Send()); }

        internal void SendSyncData()
        {
            if (!syncData.Any()) { return; }

            Dictionary<string, object> data = PullSyncData();

            if (!data.Any()) { return; }

            switch (NetManager.GetMode())
            {
                case NetManager.Mode.Offline:
                    NetManager.Client.AccessElement(PairKey, e =>
                    {
                        e.PushSyncData(data);
                    });
                    return;

                case NetManager.Mode.Client:
                    return;

                case NetManager.Mode.Server:
                    Server.Server.Send.Sync(PairKey, data); 
                    return;
            }
        }

        protected void WriteSync()
        {
            if (NetManager.GetMode() == NetManager.Mode.Client) { return; }
            foreach (FieldInfo field in GetSyncedMembers())
            {
                SyncAttribute attribute = field.GetCustomAttribute<SyncAttribute>();
                string key = attribute.Key ?? field.Name;
                if (syncData.ContainsKey(key)) { syncData[key].Set(field.GetValue(this)); } 
                else { syncData.Add(key, new SyncData(field.GetValue(this))); }
            }
        }

        protected void ReadSync()
        {
            if (NetManager.GetMode() == NetManager.Mode.Server) { return; }
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