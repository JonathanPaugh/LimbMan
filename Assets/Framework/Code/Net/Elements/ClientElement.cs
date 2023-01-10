using System;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class ClientElement : NetElement
    {
        internal override NetMode NetSide => NetMode.Client;
        protected override Communication CommuncationMode => Communication.All;
        protected override Type PairType => typeof(ServerElement);

        protected sealed override void ReceiveStream(NetStream.ServerReader stream) {}
        protected sealed override void SendStream(NetStream.ServerWriter stream) {}

        protected override void WriteSync() {}
    }
}