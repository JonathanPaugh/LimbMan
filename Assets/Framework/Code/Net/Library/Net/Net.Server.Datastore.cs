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
            public static class Datastore
            {
                public static void Test()
                {
                    Insert("Test", "Test", "{ 'UnityKey1': 'UnityValue1', 'UnityKey2': 'UnityValue2', 'UnityKey3': 'UnityValue3' }", key =>
                    {
                        Update("Test", "Test", key, "{ 'UnityKey1': 'UnityUpdate1', 'UnityKey3': 'UnityUpdate3' }",  _ =>
                        {
                            Get("Test", "Test", key, _ =>
                            {
                                Remove("Test", "Test", key, new [] { "UnityKey2" }, _ =>
                                {
                                    Delete("Test", "Test", key, data =>
                                    {
                                        Log.Write(data);
                                    });
                                });
                            });
                        });
                    });
                }

                public static void Get(string store, string collection, string key, Action<string> response)
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
                            JapeNet.Datastore.Get(store, collection, key).Read(data =>
                            {
                                response?.Invoke(data);
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

                public static void Insert(string store, string collection, string data, Action<string> response)
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
                            JapeNet.Datastore.Insert(store, collection, data).Read(data =>
                            {
                                response?.Invoke(data);
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

                public static void Update(string store, string collection, string key, string data, Action<string> response)
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
                            JapeNet.Datastore.Update(store, collection, key, data).Read(data =>
                            {
                                response?.Invoke(data);
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

                public static void Remove(string store, string collection, string key, string[] data, Action<string> response)
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
                            JapeNet.Datastore.Remove(store, collection, key, data).Read(data =>
                            {
                                response?.Invoke(data);
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

                public static void Delete(string store, string collection, string key, Action<string> response)
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
                            JapeNet.Datastore.Delete(store, collection, key).Read(data =>
                            {
                                response?.Invoke(data);
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