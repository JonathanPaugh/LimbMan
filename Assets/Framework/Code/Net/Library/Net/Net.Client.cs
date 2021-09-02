using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Client
        {
            public static void Invoke(string value, params object[] args)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Client:
                    {
                        JapeNet.Client.Client.Send.Invoke(value, args);
                        return;
                    }

                    default:
                    {
                        ServerAccessError();
                        return;
                    }
                }
            }

            private static void ServerAccessError()
            {
                Log.Write("Server cannot call server commands");
            }
        }
    }
}