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
            public static class NetTable
            {
                public static void RequestGlobal(string value, Action<object> action)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            action.Invoke(JapeNet.Server.Server.NetTable.Get(0, value));
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            Request(0, value, action);
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }

                public static void Request(int player, string value, Action<object> action)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            action.Invoke(JapeNet.Server.Server.NetTable.Get(player, value));
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            JapeNet.Client.Client.Send.Request(player, value, action);
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }

                    
                }

                public static void ListenGlobal(string value, Action<object> action)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Server.Server.NetTable.ListenStart(JapeNet.Client.Client.server.id, 
                                                                       0, 
                                                                       value,
                                                                       JapeNet.Client.Client.NetListener.Open(action));
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            Listen(0, value, action);
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }

                public static void Listen(int player, string value, Action<object> action)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Server.Server.NetTable.ListenStart(JapeNet.Client.Client.server.id, 
                                                                       player, 
                                                                       value,
                                                                       JapeNet.Client.Client.NetListener.Open(action));
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            JapeNet.Client.Client.Send.ListenStart(player, value, action);
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }

                public static void UnlistenGlobal(string value)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Server.Server.NetTable.ListenStop(JapeNet.Client.Client.server.id,
                                                                      0,
                                                                      value);
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            Unlisten(0, value);
                            return;
                        }

                        default:
                        {
                            ServerAccessError();
                            return;
                        }
                    }
                }

                public static void Unlisten(int player, string value)
                {
                    switch (NetManager.GetMode())
                    {
                        case NetManager.Mode.Offline:
                        {
                            JapeNet.Server.Server.NetTable.ListenStop(JapeNet.Client.Client.server.id,
                                                                      player,
                                                                      value);
                            return;
                        }

                        case NetManager.Mode.Client:
                        {
                            JapeNet.Client.Client.Send.ListenStop(player, value);
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