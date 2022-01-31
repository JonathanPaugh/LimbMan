using System;
using System.Collections.Generic;
using System.Net;
using Jape;

namespace JapeNet.Client
{
    public partial class Client
    {
        public static class Receive
        {
            public static Dictionary<Server.Server.Packets, PacketHandler> packets = new Dictionary<Server.Server.Packets, PacketHandler>
            {
                { Server.Server.Packets.Register, Register },
                { Server.Server.Packets.VerifyTcp, VerifyTcp },
                { Server.Server.Packets.VerifyUdp, VerifyUdp },
                { Server.Server.Packets.Connect, Connect },
                { Server.Server.Packets.Pong, Pong },
                { Server.Server.Packets.Suspend, Suspend },
                { Server.Server.Packets.Restore, Restore },
                { Server.Server.Packets.Field, Field },
                { Server.Server.Packets.Call, Call },
                { Server.Server.Packets.Stream, Stream },
                { Server.Server.Packets.Sync, Sync },
                { Server.Server.Packets.Response, Response },
                { Server.Server.Packets.ResponseClose, ResponseClose },
                { Server.Server.Packets.PlayerConnect, PlayerConnect },
                { Server.Server.Packets.PlayerDisconnect, PlayerDisconnect },
                { Server.Server.Packets.Spawn, Spawn },
                { Server.Server.Packets.Despawn, Despawn },
                { Server.Server.Packets.Parent, Parent },
                { Server.Server.Packets.SetActive, SetActive },
                { Server.Server.Packets.SceneChange, SceneChange }
            };
          
            internal static void Register(Packet packet)
            {
                server.id = packet.ReadInt();

                Log.Write($"Registered: Player {server.id}");

                Send.Registered();
            }

            internal static void VerifyTcp(Packet packet)
            {
                if (server.connectedTcp) { return; }

                server.connectedTcp = true;

                Log.Write("Verified Tcp");

                if (server.mode == Connection.Mode.Default)
                {
                    server.udp.Start(((IPEndPoint)server.tcp.socket.Client.LocalEndPoint).Port);
                }

                Send.VerifiedTcp();
            }

            internal static void VerifyUdp(Packet packet)
            {
                if (server.connectedUdp) { return; }

                server.connectedUdp = true;

                Log.Write("Verified Udp");

                Send.VerifiedUdp();
            }

            internal static void Connect(Packet packet)
            {
                server.connected = true;

                Log.Write("Connected");

                Send.Connected();

                server.Connected();

                NetManager.ConnectClient(server.id);
            }

            internal static void Pong(Packet packet)
            {
                DateTime time = DateTime.Parse(packet.ReadString());
                int ping = (int)(DateTime.UtcNow - time).TotalMilliseconds;
                server.SetLatencyMetrics(ping);
            }

            internal static void Suspend(Packet packet)
            {
                server.suspended = true;

                Log.Write("Suspended");
            }

            internal static void Restore(Packet packet)
            {
                server.suspended = false;

                Log.Write("Restored");
            }

            internal static void Field(Packet packet)
            {
                byte[] key = packet.ReadMonoKey();
                string name = packet.ReadString();

                NetManager.Client.AccessElement(key, e =>
                {
                    Member.Set(e, name, packet.ReadObject());
                });
            }

            internal static void Call(Packet packet)
            {
                byte[] key = packet.ReadMonoKey();
                string name = packet.ReadString();

                NetManager.Client.AccessElement(key, e =>
                {
                    e.ReceiveCall(name, packet.ReadObjects());
                });
            }

            internal static void Stream(Packet packet)
            {
                byte[] key = packet.ReadMonoKey();

                NetManager.Client.AccessElement(key, e =>
                {
                    e.PushStreamData(packet.ReadObjects());
                });
            }

            internal static void Sync(Packet packet)
            {
                byte[] key = packet.ReadMonoKey();

                Dictionary<string, object> data = new Dictionary<string, object>();

                int length = packet.ReadInt();
                for (int i = 0; i < length; i++)
                {
                    data.Add(packet.ReadString(), packet.ReadObject());
                }

                NetManager.Client.AccessElement(key, e =>
                {
                    e.PushSyncData(data);
                });
            }

            internal static void Response(Packet packet)
            {
                int index = packet.ReadInt();

                NetListener.Receive(index).Invoke(packet.ReadObject());

                if (packet.ReadBool())
                {
                    NetListener.Close(index);
                }
            }

            internal static void ResponseClose(Packet packet)
            {
                NetListener.Close(packet.ReadInt());
            }

            internal static void PlayerConnect(Packet packet)
            {
                int client = packet.ReadInt();

                Log.Write($"Connected: Player {client}");

                NetManager.PlayerConnectClient(client);
            }

            internal static void PlayerDisconnect(Packet packet)
            {
                int client = packet.ReadInt();

                Log.Write($"Disconnected: Player {client}");

                NetManager.PlayerDisconnectClient(client);
            }

            internal static void Spawn(Packet packet)
            {
                NetManager.Spawn(packet.ReadString(),
                                 packet.ReadString(),
                                 packet.ReadVector3(), 
                                 packet.ReadQuaternion(), 
                                 packet.ReadString(), 
                                 packet.ReadInt(),
                                 packet.ReadBool(),
                                 packet.ReadBool());
            }

            internal static void Despawn(Packet packet)
            {
                NetManager.Despawn(packet.ReadString());
            }

            internal static void Parent(Packet packet)
            {
                NetManager.Parent(packet.ReadString(), packet.ReadString());
            }

            internal static void SetActive(Packet packet)
            {
                NetManager.SetActive(packet.ReadString(), packet.ReadBool());
            }

            internal static void SceneChange(Packet packet)
            {
                NetManager.SceneChange(packet.ReadString(), Send.SceneChanged);
            }
        }
    }
}
