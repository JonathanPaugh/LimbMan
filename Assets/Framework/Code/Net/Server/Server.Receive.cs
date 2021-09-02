using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.Serialization;
using UnityEngine;

namespace JapeNet.Server
{
    public partial class Server
    {
        public static class Receive
        {
            public static Dictionary<Client.Client.Packets, PacketHandler> packets = new Dictionary<Client.Client.Packets, PacketHandler>
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
                { Client.Client.Packets.Request, Request },
                { Client.Client.Packets.ListenStart, ListenStart },
                { Client.Client.Packets.ListenStop, ListenStop },
                { Client.Client.Packets.Invoke, Invoke }
            };

            internal static void Registered(int id, Packet packet)
            {
                int check = packet.ReadInt();
                Log.Write($"Registered Player {id}: {Clients[id].tcp.socket.Client.RemoteEndPoint}");
                if (id != check) { Log.Write($"Id Conflict: ({id})({check})"); };
                Clients[id].mode = !packet.ReadBool() ? Connection.Mode.Default : Connection.Mode.Web;
                Clients[id].Verify();
            }

            internal static void VerifiedTcp(int id, Packet packet)
            {
                Clients[id].connectedTcp = true;
                Log.Write($"Verified Tcp: {Clients[id].tcp.socket.Client.RemoteEndPoint}");
            }

            internal static void VerifiedUdp(int id, Packet packet)
            {
                Clients[id].connectedUdp = true;
                Log.Write($"Verified Udp: {Clients[id].tcp.socket.Client.RemoteEndPoint}");
            }

            internal static void Connected(int id, Packet packet)
            {
                Clients[id].connected = true;
                Log.Write($"Connected: {Clients[id].tcp.socket.Client.RemoteEndPoint}");
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
                string key = packet.ReadString();
                string name = packet.ReadString();

                NetManager.Server.AccessElement(id, key, e =>
                {
                    Member.Set(e, name, packet.ReadObject());
                });
            }

            internal static void Call(int id, Packet packet)
            {
                string key = packet.ReadString();
                string name = packet.ReadString();

                NetManager.Server.AccessElement(id, key, e =>
                {
                    e.ReceiveCall(name, packet.ReadObjects());
                });
            }

            internal static void Stream(int id, Packet packet)
            {
                string key = packet.ReadString();

                NetManager.Server.AccessElement(id, key, e =>
                {
                    e.PushStreamData(packet.ReadObjects());
                });
            }

            internal static void Request(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();
                int index = packet.ReadInt();

                Send.Response(id, index, NetTable.Get(player, key), true);
            }

            internal static void ListenStart(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();
                int index = packet.ReadInt();

                NetTable.ListenStart(id, player, key, index);
            }

            internal static void ListenStop(int id, Packet packet)
            {
                int player = packet.ReadInt();
                string key = packet.ReadString();

                NetTable.ListenStop(id, player, key);
            }

            internal static void Invoke(int id, Packet packet)
            {
                string key = packet.ReadString();

                NetDelegator.Invoke(key, packet.ReadObjects());
            }
        }
    }
}