using System;
using System.Globalization;
using Jape;

namespace JapeNet.Client
{
    public partial class Client
    {
        public enum Packets
        {
            _CONNECT_PACKETS_ = 1,
            Registered,
            VerifiedTcp,
            VerifiedUdp,
            Connected,
            _SYSTEM_PACKETS_,
            Ping,
            SceneChanged,
            _DATA_PACKETS_,
            Field,
            Call,
            Stream,
            Request,
            ListenStart,
            ListenStop,
            Invoke
        }

        public static class Send
        {
            internal static void Registered()
            {
                using (Packet packet = new Packet((int)Packets.Registered))
                {
                    packet.Write(server.id);
                    packet.Write(server.mode != Connection.Mode.Default);
                    SendTcp(packet);
                }
            }

            internal static void VerifiedTcp()
            {
                using (Packet packet = new Packet((int)Packets.VerifiedTcp))
                {
                    SendTcp(packet);
                }
            }

            internal static void VerifiedUdp()
            {
                using (Packet packet = new Packet((int)Packets.VerifiedUdp))
                {
                    SendUdp(packet);
                }
            }

            internal static void Connected()
            {
                using (Packet packet = new Packet((int)Packets.Connected))
                {
                    SendTcp(packet);
                }
            }

            internal static void Ping()
            {
                using (Packet packet = new Packet((int)Packets.Ping))
                {
                    packet.Write(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));

                    SendTcp(packet);
                }
            }

            internal static void SceneChanged()
            {
                using (Packet packet = new Packet((int)Packets.SceneChanged))
                {
                    SendTcp(packet);
                }
            }

            internal static void Field(Element.Key elementKey, string name, object value)
            {
                using (Packet packet = new Packet((int)Packets.Field))
                {
                    packet.Write(elementKey);
                    packet.Write(name);
                    packet.Write(value);

                    SendTcp(packet);
                }
            }

            internal static void Call(Element.Key elementKey, string name, params object[] args)
            {
                using (Packet packet = new Packet((int)Packets.Call))
                {
                    packet.Write(elementKey);
                    packet.Write(name);
                    packet.Write(args);

                    SendTcp(packet);
                }
            }

            internal static void Stream(Element.Key elementKey, object[] streamData)
            {
                using (Packet packet = new Packet((int)Packets.Stream))
                {
                    packet.Write(elementKey);
                    packet.Write(streamData);

                    SendUdp(packet);
                }
            }

            internal static void Request(int player, string key, Action<object> action)
            {
                using (Packet packet = new Packet((int)Packets.Request))
                {
                    packet.Write(player);
                    packet.Write(key);
                    packet.Write(NetListener.Open(action));

                    SendTcp(packet);
                }
            }

            internal static int ListenStart(int player, string key, Action<object> action)
            {
                using (Packet packet = new Packet((int)Packets.ListenStart))
                {
                    packet.Write(player);
                    packet.Write(key);

                    int index = NetListener.Open(action);
                    packet.Write(index);

                    SendTcp(packet);

                    return index;
                }
            }

            internal static void ListenStop(int player, string key)
            {
                using (Packet packet = new Packet((int)Packets.ListenStop))
                {
                    packet.Write(player);
                    packet.Write(key);

                    SendTcp(packet);
                }
            }

            internal static void Invoke(string key, object[] args)
            {
                using (Packet packet = new Packet((int)Packets.Invoke))
                {
                    packet.Write(key);
                    packet.Write(args);

                    SendTcp(packet);
                }
            }
        }
    }
}