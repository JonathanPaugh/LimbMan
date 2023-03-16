using System;

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
                    if (Mode.IsLocal)
                    {
                        action.Invoke(JapeNet.Server.Server.netTable.Get(0, value));
                    }
                    else if (Mode.IsClient)
                    {
                        Request(0, value, action);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }

                public static void Request(int player, string value, Action<object> action)
                {
                    if (Mode.IsLocal)
                    {
                        action.Invoke(JapeNet.Server.Server.netTable.Get(player, value));
                    }
                    else if (Mode.IsClient)
                    {
                        JapeNet.Client.Client.Send.Request(player, value, action);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }

                public static void ListenGlobal(string value, Action<object> action)
                {
                    if (Mode.IsLocal)
                    {
                        JapeNet.Server.Server.netTable.ListenStart(JapeNet.Client.Client.Id, 
                                                                   0, 
                                                                   value,
                                                                   JapeNet.Client.Client.netListener.Open(action));
                    }
                    else if (Mode.IsClient)
                    {
                        Listen(0, value, action);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }

                public static void Listen(int player, string value, Action<object> action)
                {
                    if (Mode.IsLocal)
                    {
                        JapeNet.Server.Server.netTable.ListenStart(JapeNet.Client.Client.Id, 
                                                                   player, 
                                                                   value,
                                                                   JapeNet.Client.Client.netListener.Open(action));
                    }
                    else if (Mode.IsClient)
                    {
                        JapeNet.Client.Client.Send.ListenStart(player, value, action);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }

                public static void UnlistenGlobal(string value)
                {
                    if (Mode.IsLocal)
                    {
                        JapeNet.Server.Server.netTable.ListenStop(JapeNet.Client.Client.Id,  
                                                                  0,
                                                                  value);
                    }
                    else if (Mode.IsClient)
                    {
                        Unlisten(0, value);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }

                public static void Unlisten(int player, string value)
                {
                    if (Mode.IsLocal)
                    {
                        JapeNet.Server.Server.netTable.ListenStop(JapeNet.Client.Client.Id,
                                                                  player,
                                                                  value);
                    }
                    else if (Mode.IsClient)
                    {
                        JapeNet.Client.Client.Send.ListenStop(player, value);
                    } 
                    else
                    {
                        ServerAccessError();
                    }
                }
            }
        }
    }
}