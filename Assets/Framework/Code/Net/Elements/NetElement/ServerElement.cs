using System;
using Jape;

namespace JapeNet
{
    public abstract class ServerElement : SideElement
    {
        protected override NetMode Side => NetMode.Server;
        protected override Type PairType => typeof(ClientElement);

        protected virtual int StreamRate => DefaultStreamRate;
        protected sealed override int ServerStreamRate => StreamRate;
        protected sealed override int ClientStreamRate => base.ClientStreamRate;

        protected sealed override void ReceiveStream(NetStream.IClientReader stream) {}
        protected sealed override void SendStream(NetStream.IClientWriter stream) {}

        protected new class ReceiveSyncAttribute : NetElement.ReceiveSyncAttribute
        {
            public ReceiveSyncAttribute(string key = null) : base(NetSide.Serverside, key) {}
        }

        protected new class SendSyncAttribute : NetElement.SendSyncAttribute
        {
            public SendSyncAttribute(string key = null) : base(NetSide.Serverside, key) {}
        }
    }
}