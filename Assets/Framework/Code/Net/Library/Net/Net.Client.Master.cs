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
                public static void ServerCreate(Action<Response.CreateBody> response = null, Action error = null)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Master.ServerCreate()
                                          .ReadJson(response)
                                          .Error(error);
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

                public static void ServerInfo(Action<Response.InfoBody> response = null, Action error = null)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Master.ServerInfo()
                                          .ReadJson(response)
                                          .Error(error);
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            Log.Write("Must request server info while offline");
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