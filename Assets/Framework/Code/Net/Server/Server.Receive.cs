using System.Collections.Generic;
using Jape;

namespace JapeNet.Server
{
    public partial class Server
    {
        public static class Receive
        {
            public static Dictionary<Client.Client.Packets, PacketHandler> packets = new()
            {
                { Client.Client.Packets.Registered, Registered },
                { Client.Client.Packets.VerifiedTcp, VerifiedTcp },
                { Client.Client.Packets.VerifiedUdp, VerifiedUdp },
                { Client.Client.Packets.Connected, Connected },
                { Client.Client.Packets.Ping, Ping },
                { Client.Client.Packets.SceneChanged, SceneChanged },
                { Client.Client.Packets.Field, Field },
                { Client.Client.Packets.Call, Call },
                { Client.Client.Packets.Stream, Stream },
                { Client.Client.Packets.Sync, Sync },
                { Client.Client.Packets.Request, Request },
                { Client.Client.Packets.ListenStart, ListenStart },
                { Client.Client.Packets.ListenStop, ListenStop },
                { Client.Client.Packets.Invoke, Invoke }
            };

            internal static void Registered(int id, Packet packet)
            {
                int check = packet.ReadInt();
                Log.Write($"Registered Player {id}: {clients[id].tcp.socket.Client.RemoteEndPoint}");
                if (id != check) { Log.Write($"Id Conflict: ({id})({check})"); }
                clients[id].mode = !packet.ReadBool() ? Connection.Mode.Default : Connection.Mode.Web;
                clients[id].Verify();
            }

            internal static void VerifiedTcp(int id, Packet packet)
            {
                clients[id].connectedTcp = true;
                Log.Write($"Verified Tcp: {clients[id].tcp.socket.Client.RemoteEndPoint}");
            }

            internal static void VerifiedUdp(int id, Packet packet)
            {
                clients[id].connectedUdp = true;
                Log.Write($"Verified Udp: {clients[id].tcp.socket.Client.RemoteEndPoint}");
            }

            internal static void Connected(int id, Packet packet)
            {
                clients[id].connected = true;
                Log.Write($"Connected: {clients[id].tcp.socket.Client.RemoteEndPoint}");
                Send.PlayerConnect(id);
                NetManager.PlayerConnectServer(id);
            }

            internal static void Ping(int id, Packet packet)
            {
                Send.Pong(id, packet.ReadString());
            }

            internal static void SceneChanged(int id, Packet packet)
            {
                NetManager.PlayerSceneChangeServer(id);
            }

            internal static void Field(int id, Packet packet)
            {
                byte[] key = packet.ReadMonoKey();
                string name = packet.ReadString();

                NetManager.AccessServerElement(id, key, e =>
                {
                    e.ReceiveField(name, packet.ReadObject());
                });
            }

            internal static void Call(int id, Packet packet)
            {
                byte[] key = packet.ReadMonoKey();
                string name = packet.ReadString();

                NetManager.AccessServerElement(id, key, e =>
                {
                    e.ReceiveCall(name, packet.ReadObjects());
                });
            }

            internal static void Stream(int id, Packet packet)
            {
                byte[] key = packet.ReadMonoKey();

                NetManager.AccessServerElement(id, key, e =>
                {
                    e.ReceiveStreamData(NetSide.Serverside, packet.ReadObjects());
                });
            }

            internal static void Sync(int id, Packet packet)
            {
                byte[] key = packet.ReadMonoKey();

                Dictionary<string, object> data = new();

                int length = packet.ReadInt();
                for (int i = 0; i < length; i++)
                {
                    data.Add(packet.ReadString(), packet.ReadObject());
                }

                NetManager.AccessServerElement(id, key, e =>
                {
                    e.ReceiveSyncData(NetSide.Serverside, data);
                });
            }

            internal static void Request(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();
                int index = packet.ReadInt();

                Send.Response(id, index, netTable.Get(player, key), true);
            }

            internal static void ListenStart(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();
                int index = packet.ReadInt();

                netTable.ListenStart(id, player, key, index);
            }

            internal static void ListenStop(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();

                netTable.ListenStop(id, player, key);
            }

            internal static void Invoke(int id, Packet packet)
            {
                string key = packet.ReadString();

                netDelegator.Invoke(key, packet.ReadObjects());
            }
        }
    }
}