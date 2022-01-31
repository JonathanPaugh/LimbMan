using System;
using Jape;

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
                    string insertData = "{ " 
                                        + '"' + "UnityKey1" + '"' + ": " 
                                        + '"' + "UnityValue1" + '"' + ", " 
                                        + '"' + "UnityKey2" + '"' + ": " 
                                        + '"' + "UnityValue2" + '"' + ", " 
                                        + '"' + "UnityKey3" + '"' + ": " 
                                        + '"' + "UnityValue3" + '"' 
                                        + " }";

                    string updateData = "{ " 
                                        + '"' + "UnityKey1" + '"' + ": " 
                                        + '"' + "UnityUpdate1" + '"' + ", " 
                                        + '"' + "UnityKey3" + '"' + ": " 
                                        + '"' + "UnityUpdate3" + '"' 
                                        + " }";

                    Insert("Test", "Test", insertData, id =>
                    {
                        Update("Test", "Test", id.oid, updateData, _ =>
                        {
                            Get("Test", "Test", id.oid, _ =>
                            {
                                Remove("Test", "Test", id.oid, new [] { "UnityKey2" }, _ =>
                                {
                                    Delete("Test", "Test", id.oid, data =>
                                    {
                                        Log.Write(data);
                                    });
                                });
                            }, () => Log.Write("Error"));
                        });
                    });
                }

                public static void Get(string store, string collection, string key, Action<string> response = null, Action error = null)
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
                            JapeNet.Datastore.Get(store, collection, key)
                                             .Read(response)
                                             .Error(error);
                            return;
                        }

                        default:
                        {
                            ClientAccessError();
                            return;
                        }
                    }
                }

                public static void Insert(string store, string collection, string data, Action<Response.IdBody> response = null, Action error = null)
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
                            JapeNet.Datastore.Insert(store, collection, data)
                                             .ReadJson(response, json => json.Replace("$oid", "oid"))
                                             .Error(error);
                            return;
                        }

                        default:
                        {
                            ClientAccessError();
                            return;
                        }
                    }
                }

                public static void Update(string store, string collection, string key, string data, Action<string> response = null, Action error = null)
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
                            JapeNet.Datastore.Update(store, collection, key, data)
                                             .Read(response)
                                             .Error(error);
                            return;
                        }

                        default:
                        {
                            ClientAccessError();
                            return;
                        }
                    }
                }

                public static void Remove(string store, string collection, string key, string[] data, Action<string> response = null, Action error = null)
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
                            JapeNet.Datastore.Remove(store, collection, key, data)
                                             .Read(response)
                                             .Error(error);
                            return;
                        }

                        default:
                        {
                            ClientAccessError();
                            return;
                        }
                    }
                }

                public static void Delete(string store, string collection, string key, Action<string> response = null, Action error = null)
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
                            JapeNet.Datastore.Delete(store, collection, key)
                                             .Read(response)
                                             .Error(error);
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