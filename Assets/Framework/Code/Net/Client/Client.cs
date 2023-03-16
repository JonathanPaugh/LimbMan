using System;
using System.Net;
using Jape;

namespace JapeNet.Client
{
    public static partial class Client
    {
        private static NetSettings Settings => NetManager.Settings;

        private static IPAddress Ip => IPAddress.Parse(Settings.ServerIp);
        private static int Port => Settings.serverPort;
        private static int BufferSize => Settings.bufferSize;
        private static bool TcpBatching => Settings.tcpBatching;

        internal static Connection server;
        internal static readonly NetListener netListener = new();

        public static int Id => server?.id ?? 0;
        public static bool Connected => server?.connected ?? false;

        public static bool IsRemote(int client) { return client != Id; }

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
            using (Packet dataPacket = new(data))
            {
                dataPacket.ReadInt();
                return dataPacket.ReadInt() < (int)Packets._DATA_PACKETS_;
            }
        }

        public static bool IsDataPacket(byte[] data)
        {
            using (Packet dataPacket = new(data))
            {
                dataPacket.ReadInt();
                return dataPacket.ReadInt() > (int)Packets._DATA_PACKETS_;
            }
        }
    }
}