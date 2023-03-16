using Jape;

namespace JapeNet
{
	internal static class Datacache
    {
        private static NetSettings Settings => NetManager.Settings;

        private static Protocol Protocol => Settings.databaseProtocol;
        private static string Ip => Settings.databaseIp;
        private static int Port => Settings.databasePort;

        private static string Url => $"{Protocol.ToString().ToLowerInvariant()}://{Ip}:{Port}";

        private static Response Response(Request.DatacacheBody body)
        {
            Request request = Request.Post
            (
                Url,
                body.ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Get(string key) => Response(new Request.Datacache.GetBody(key));
        internal static Response Set(string key, string value) => Response(new Request.Datacache.SetBody(key, value));
        internal static Response Remove(string key) => Response(new Request.Datacache.RemoveBody(key));
        internal static Response Subscribe(string channel, Request.Datacache.SubscribeBody.Mode mode) => Response(new Request.Datacache.SubscribeBody(channel, mode));
        internal static Response Unsubscribe(string subscription) => Response(new Request.Datacache.UnsubscribeBody(subscription));
        internal static Response Publish(string channel, string value) => Response(new Request.Datacache.PublishBody(channel, value));
        internal static Response Receive(string subscription) => Response(new Request.Datacache.ReceiveBody(subscription));
    }
}