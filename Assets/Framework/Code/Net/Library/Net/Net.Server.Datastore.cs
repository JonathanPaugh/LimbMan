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
                        Update("Test", "Test", id.oid, updateData, _1 =>
                        {
                            Get("Test", "Test", id.oid, _2 =>
                            {
                                Remove("Test", "Test", id.oid, new [] { "UnityKey2" }, _3 =>
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
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datastore.Get(store, collection, key)
                                     .Read(response)
                                     .Error(error);
                }

                public static void Insert(string store, string collection, string data, Action<Response.IdBody> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datastore.Insert(store, collection, data)
                                     .ReadJson(response, json => json.Replace("$oid", "oid"))
                                     .Error(error);
                }

                public static void Update(string store, string collection, string key, string data, Action<string> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datastore.Update(store, collection, key, data)
                                     .Read(response)
                                     .Error(error);
                }

                public static void Remove(string store, string collection, string key, string[] data, Action<string> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datastore.Remove(store, collection, key, data)
                                     .Read(response)
                                     .Error(error);
                }

                public static void Delete(string store, string collection, string key, Action<string> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datastore.Delete(store, collection, key)
                                     .Read(response)
                                     .Error(error);
                }
            }
        }
    }
}