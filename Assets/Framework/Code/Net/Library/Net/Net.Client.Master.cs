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
            public static class Master
            {
                public static void ServerCreate(Action<string> response)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Master.ServerCreate().Read(data =>
                            {
                                response?.Invoke(data);
                            });
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            Log.Write("Must request server while offline");
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }

                public static void ServerInfo(Action<Response.InfoBody> response)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Master.ServerInfo().ReadJson<Response.InfoBody>(data =>
                            {
                                response?.Invoke(data);
                            });
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            JapeNet.Master.ServerInfo();
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }
            }
        }
    }
}