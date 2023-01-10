using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Jape;
using Sirenix.Utilities;
using UnityEngine;

namespace JapeNet.Server
{
    public static partial class Server
    {
        private static NetSettings Settings => NetManager.Settings;

        private static int MaxPlayers => Settings.maxPlayers;
        private static int Port => Settings.serverPort;
        private static int BufferSize => Settings.bufferSize;
        private static bool TcpBatching => Settings.tcpBatching;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static readonly NetTable netTable = new NetTable();
        public static readonly NetDelegator netDelegator = new NetDelegator();
        private static readonly Dictionary<int, Connection> clients = new Dictionary<int, Connection>();

        public delegate void PacketHandler(int id, Packet packet);

        public static bool IsEmpty() { return GetConnectedClients().Length == 0; }

        public static int[] GetPlayers() { return clients.Values.Where(c => c.connected).Select(c => c.id).ToArray(); }
        public static Connection[] GetConnectedClients() { return clients.Values.Where(c => c.connected).ToArray(); }
        public static Connection[] GetRemoteClients() { return GetConnectedClients().Where(c => Client.Client.IsRemote(c.id)).ToArray(); }
        public static Connection GetClient(int id) { return clients[id]; }

        internal static void Init()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Connection(i));
            }
        }

        public static void Start(Action success, Action error)
        {
            Log.Write("Starting Server", $"Port: {Port}");

            try
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(ConnectTcp, null);

                udpListener = new UdpClient(Port);
                udpListener.BeginReceive(ConnectUdp, null);
            }
            catch
            {
                error?.Invoke();
                return;
            }

            success?.Invoke();

            NetManager.StartServer();
        }

        public static void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();

            Log.Write("Server Stopped");

            NetManager.StopServer();
        }

        private static void ConnectTcp(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(ConnectTcp, null);

            ThreadManager.QueueFrame(() => Log.Write($"Incoming Connection: {client.Client.RemoteEndPoint}"));

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].Connect(client);
                    return;
                }
            }

            ThreadManager.QueueFrame(() => Log.Write("Server Full"));
        }

        private static void SendTcp(int id, Packet packet)
        {
            packet.InsertLength();
            clients[id].tcp.Send(packet);
        }

        private static void SendTcpAll(Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients[i].tcp.Send(packet);
            }
        }

        private static void SendTcpExcluding(int id, Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (i != id)
                {
                    clients[i].tcp.Send(packet);
                }
            }
        }

        private static void ConnectUdp(IAsyncResult result)
        {
            try
            {
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref client);
                udpListener.BeginReceive(ConnectUdp, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int id = packet.ReadInt();

                    if (id == 0)
                    {
                        ThreadManager.QueueFrame(() => Log.Write("Client Unregistered"));
                        return;
                    }

                    if (clients[id].udp.ip == null)
                    {
                        clients[id].udp.Start(client);
                        return;
                    }

                    if (clients[id].udp.ip.ToString() == client.ToString())
                    {
                        clients[id].udp.Read(packet);
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.QueueFrame(() => Log.Write($"Error receiving udp data: {e}"));
            }
        }

        private static void SendUdp(int id, Packet packet)
        {
            packet.InsertLength();

            switch (clients[id].mode)
            {
                case Connection.Mode.Default:
                    clients[id].udp.Send(packet);
                    break;

                case Connection.Mode.Web:
                    clients[id].tcp.Send(packet);
                    break;
            }
        }

        private static void SendUdpAll(Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                switch (clients[i].mode)
                {
                    case Connection.Mode.Default:
                        clients[i].udp.Send(packet);
                        break;

                    case Connection.Mode.Web:
                        clients[i].tcp.Send(packet);
                        break;
                }
            }
        }

        private static void SendUdpExcluding(int id, Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (i == id) { continue; }
                switch (clients[id].mode)
                {
                    case Connection.Mode.Default:
                        clients[id].udp.Send(packet);
                        break;

                    case Connection.Mode.Web:
                        clients[id].tcp.Send(packet);
                        break;
                }
            }
        }

        public static bool IsConnectPacket(byte[] data)
        {
            using (Packet dataPacket = new Packet(data))
            {
                dataPacket.ReadInt();
                return dataPacket.ReadInt() < (int)Packets._SYSTEM_PACKETS_;
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
