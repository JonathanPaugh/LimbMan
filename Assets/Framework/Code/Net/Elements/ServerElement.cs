using System;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class ServerElement : NetElement
    {
        protected override NetManager.Mode Mode => NetManager.Mode.Server;
        protected override Type PairType => typeof(ClientElement);

        protected sealed override int ClientStreamRate => base.ClientStreamRate;

        internal override void FixedUpdate()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Offline)
                {
                    ReadStream(Mode);
                }
            }

            base.FixedUpdate();

            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Offline)
                {
                    IncrementStream(Mode);
                    WriteStream(Mode);
                    SendStreamData();

                    WriteSync();
                    SendSyncData();
                }
            }
        }

        protected sealed override void ReceiveStream(NetStream.ClientReader stream) {}
        protected sealed override void SendStream(NetStream.ClientWriter stream) {}
    }
}