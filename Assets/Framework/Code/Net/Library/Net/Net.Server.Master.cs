using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Server
        {
            public static class Master
            {
                public static void ServerDestroy(Action response)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            OfflineAccessError();
                            return;
                        }

                        case NetManager.Mode.Server:
                        {
                            JapeNet.Master.ServerDestroy().Read(_ =>
                            {
                                response?.Invoke();
                            });
                            return;
                        }

                        default:
                        {
                            ClientAccessError();
                            return;
                        }
                    }
                }
            }
        }
    }
}