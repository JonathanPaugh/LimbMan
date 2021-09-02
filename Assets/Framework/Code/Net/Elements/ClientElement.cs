using System;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class ClientElement : NetElement
    {
        protected override NetManager.Mode Mode => NetManager.Mode.Client;
        protected override Type PairType => typeof(ServerElement);

        protected sealed override int ServerStreamRate => base.ServerStreamRate;

        internal override void Update()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Offline)
                {
                    ReadStream(Mode);
                    ReadSync();
                }
            }

            base.Update();

            if (Game.IsRunning)
            {
                if (NetManager.GetMode() == NetManager.Mode.Offline)
                {
                    IncrementStream(Mode);
                    WriteStream(Mode);
                    SendStreamData();
                }
            }
        }

        protected sealed override void ReceiveStream(NetStream.ServerReader stream) {}
        protected sealed override void SendStream(NetStream.ServerWriter stream) {}
    }
}