using Jape;

namespace JapeNet
{
	internal static class Master
    {
        private static NetSettings Settings => NetManager.Settings;
        
        private static Protocol Protocol => Settings.masterServerProtocol;
        private static string Ip => Settings.masterServerIp;
        private static int Port => Settings.masterServerPort;

        private static string Url => $"{Protocol.ToString().ToLowerInvariant()}://{Ip}:{Port}";

        private static Response Response(Request.Body body)
        {
            Request request = Request.Post
            (
                Url,
                body.ToJson()
            );
            return request.GetResponse();
        }

        internal static Response ServerCreate()
        {
            Request request = Request.Create(Url);
            return request.GetResponse();
        }

        internal static Response ServerInfo() => Response(new Request.ClientBody("Info"));
        internal static Response ServerActivate() => Response(new Request.ServerBody("Activate"));

        internal static Response ServerDestroy()
        {
            Request request = Request.Delete
            (
                Url,
                Settings.MasterKey
            );
            return request.GetResponse();
        }

        internal static Response PlayerConnect() => Response(new Request.ServerBody("Connect"));
        internal static Response PlayerDisconnect() => Response(new Request.ServerBody("Disconnect"));
    }
}