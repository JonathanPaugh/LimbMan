using System;
using System.Collections.Generic;
using System.Net;
using Jape;

namespace JapeNet.Client
{
    public static partial class Client
    {
        public static IPAddress Ip => IPAddress.Parse(NetManager.Settings.ServerIp);
        public static int Port = NetManager.Settings.serverPort;
        public static int BufferSize = NetManager.Settings.bufferSize;

        public static Connection server;

        public static NetListener NetListener { get; } = new NetListener();

        public delegate void PacketHandler(Packet packet);

        internal static void Init()
        {
            server = new Connection();
        }

        public static void Start(Action success, Action error, int timeout)
        {
            Log.Write("Starting Client");

            server.Connect(success, error, timeout);

            NetManager.StartClient();
        }

        public static void Stop()
        {
            server.Disconnect();
        }

        private static void SendTcp(Packet packet)
        {
            packet.InsertLength();
            switch (server.mode)
            {
                case Connection.Mode.Default:
                    server.tcp.Send(packet);
                    break;

                case Connection.Mode.Web:
                    server.web.Send(packet);
                    break;
            }
        }

        private static void SendUdp(Packet packet)
        {
            packet.InsertLength();
            switch (server.mode)
            {
                case Connection.Mode.Default:
                    server.udp.Send(packet);
                    break;

                case Connection.Mode.Web:
                    server.web.Send(packet);
                    break;
            }
        }

        public static bool IsConnectPacket(byte[] data)
        {
            using (Packet dataPacket = new Packet(data))
            {
                dataPacket.ReadInt();
                return dataPacket.ReadInt() < (int)Packets._DATA_PACKETS_;
            }
        }

        public static bool IsDataPacket(byte[] data)
        {
            using (Packet dataPacket = new Packet(data))
            {
                dataPacket.ReadInt();
                return dataPacket.ReadInt() > (int)Packets._DATA_PACKETS_;
            }
        }
    }
}