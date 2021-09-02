using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Jape;
using UnityEngine;

namespace JapeNet.Server
{
    public static partial class Server
    {
        public static int MaxPlayers => NetManager.Settings.maxPlayers;
        public static int Port => NetManager.Settings.serverPort;
        public static int BufferSize = NetManager.Settings.bufferSize;

        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static Dictionary<int, Connection> Clients { get; } = new Dictionary<int, Connection>();

        public static NetTable NetTable { get; } = new NetTable();
        public static NetDelegator NetDelegator { get; } = new NetDelegator();

        public delegate void PacketHandler(int id, Packet packet);

        public static bool IsEmpty() { return GetConnectedClients().Length == 0; }
        
        public static int[] GetPlayers() { return Clients.Values.Where(c => c.connected).Select(c => c.id).ToArray(); }
        internal static Connection[] GetConnectedClients() { return Clients.Values.Where(c => c.connected).ToArray(); }

        internal static void Init()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Connection(i));
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
                if (Clients[i].tcp.socket == null)
                {
                    Clients[i].Connect(client);
                    return;
                }
            }

            ThreadManager.QueueFrame(() => Log.Write("Server Full"));
        }

        private static void SendTcp(int id, Packet packet)
        {
            packet.InsertLength();
            Clients[id].tcp.Send(packet);
        }

        private static void SendTcpAll(Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients[i].tcp.Send(packet);
            }
        }

        private static void SendTcpExcluding(int id, Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (i != id)
                {
                    Clients[i].tcp.Send(packet);
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
                        ThreadManager.QueueFrame(() => Debug.Log("Client Unregistered"));
                        return;
                    }

                    if (Clients[id].udp.ip == null)
                    {
                        Clients[id].udp.Start(client);
                        return;
                    }

                    if (Clients[id].udp.ip.ToString() == client.ToString())
                    {
                        Clients[id].udp.Read(packet);
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

            switch (Clients[id].mode)
            {
                case Connection.Mode.Default:
                    Clients[id].udp.Send(packet);
                    break;

                case Connection.Mode.Web:
                    Clients[id].tcp.Send(packet);
                    break;
            }
        }

        private static void SendUdpAll(Packet packet)
        {
            packet.InsertLength();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                switch (Clients[i].mode)
                {
                    case Connection.Mode.Default:
                        Clients[i].udp.Send(packet);
                        break;

                    case Connection.Mode.Web:
                        Clients[i].tcp.Send(packet);
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
                switch (Clients[id].mode)
                {
                    case Connection.Mode.Default:
                        Clients[id].udp.Send(packet);
                        break;

                    case Connection.Mode.Web:
                        Clients[id].tcp.Send(packet);
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
