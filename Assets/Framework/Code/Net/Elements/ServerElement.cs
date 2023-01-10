using System;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class ServerElement : NetElement
    {
        internal override NetMode NetSide => NetMode.Server;
        protected override Communication CommuncationMode => Communication.All;
        protected override Type PairType => typeof(ClientElement);

        protected sealed override void ReceiveStream(NetStream.ClientReader stream) {}
        protected sealed override void SendStream(NetStream.ClientWriter stream) {}

        protected override void ReadSync() {}
    }
}