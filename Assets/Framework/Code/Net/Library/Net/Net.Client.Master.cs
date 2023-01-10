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
                    if (Mode.IsOnline) { OnlineAccessError(); return; }

                    JapeNet.Master.ServerCreate()
                                  .ReadJson(response)
                                  .Error(error);
                }

                public static void ServerInfo(Action<Response.InfoBody> response = null, Action error = null)
                {
                    if (Mode.IsOnline) { OnlineAccessError(); return; }

                    JapeNet.Master.ServerInfo()
                                  .ReadJson(response)
                                  .Error(error);
                }

                private static void OnlineAccessError()
                {
                    Log.Write("Must be offline to call client master commands");
                }
            }
        }
    }
}