using System;
using Jape;

namespace JapeNet
{
    public abstract class ClientElement : SideElement
    {
        protected override NetMode Side => NetMode.Client;
        protected override Type PairType => typeof(ServerElement);

        protected virtual int StreamRate => DefaultStreamRate;
        protected sealed override int ClientStreamRate => StreamRate;
        protected sealed override int ServerStreamRate => base.ServerStreamRate;

        protected sealed override void ReceiveStream(NetStream.IServerReader stream) {}
        protected sealed override void SendStream(NetStream.IServerWriter stream) {}

        protected new class ReceiveSyncAttribute : NetElement.ReceiveSyncAttribute
        {
            public ReceiveSyncAttribute(string key = null) : base(NetSide.Clientside, key) {}
        }

        protected new class SendSyncAttribute : NetElement.SendSyncAttribute
        {
            public SendSyncAttribute(string key = null) : base(NetSide.Clientside, key) {}
        }
    }
}