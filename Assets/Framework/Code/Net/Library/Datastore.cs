using Jape;

namespace JapeNet
{
	internal static class Datastore
    {
        private static NetSettings Settings => NetManager.Settings;

        private static Protocol Protocol => Settings.databaseProtocol;
        private static string Ip => Settings.databaseIp;
        private static int Port => Settings.databasePort;

        private static string Url => $"{Protocol.ToString().ToLowerInvariant()}://{Ip}:{Port}";

        private static Response Response(Request.DatastoreBody body)
        {
            Request request = Request.Post
            (
                Url,
                body.ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Get(string store, string collection, string key) => Response(new Request.Datastore.GetBody(store, collection, key));
        internal static Response Insert(string store, string collection, string data) => Response(new Request.Datastore.InsertBody(store, collection, data));
        internal static Response Update(string store, string collection, string key, string data) => Response(new Request.Datastore.UpdateBody(store, collection, key, data));
        internal static Response Remove(string store, string collection, string key, string[] data) => Response(new Request.Datastore.RemoveBody(store, collection, key, data));
        internal static Response Delete(string store, string collection, string key) => Response(new Request.Datastore.DeleteBody(store, collection, key));
    }
}