using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Jape;
using Debug = UnityEngine.Debug;
using Time = Jape.Time;

namespace JapeNet.Server
{
    public static partial class Server
    {
        public class Connection
        {
            internal enum Mode { Default, Web }
            internal Mode mode;

            public Tcp tcp;
            public Udp udp;

            public int id;

            private bool running;

            internal bool connected;
            internal bool connectedTcp;
            internal bool connectedUdp;

            internal bool suspended;

            private Timer verify;

            public Connection(int id)
            {
                this.id = id;

                tcp = new Tcp(this.id);
                udp = new Udp(this.id);

                verify = Timer.CreateGlobal();
            }

            public void Connect(TcpClient client)
            {
                if (running) { return; }
                running = true;

                tcp.Start(client);
            }

            private void Disconnect()
            {
                if (!running) { return; }
                running = false;

                tcp.Stop();
                connectedTcp = false;

                udp.Stop();
                connectedUdp = false;

                if (connected)
                {
                    connected = false;

                    ThreadManager.QueueFrame(() =>
                    {
                        Log.Write($"Disconnected: Player {id}");
                        Send.PlayerDisconnect(id);
                        NetManager.PlayerDisconnectServer(id);
                    });
                } 
                else
                {
                    verify.Stop();
                    ThreadManager.QueueFrame(() =>
                    {
                        Log.Write($"Connection Lost: Player {id}");
                    });
                }
            }

            internal void Verify()
            {
                const float Interval = 0.25f;

                if (Module.IsAlive(verify)) { Log.Write("Connection verification already in progress"); return; }

                SendPackets();

                verify.Set(Client.Client.Connection.Timeout, Time.Counter.Realtime, Interval)
                      .IntervalAction(SendPackets)
                      .CompletedAction(Timeout)
                      .Start();

                void SendPackets()
                {
                    if (Connected()) { Success(); return; }

                    switch (mode)
                    {
                        case Mode.Default:
                            if (!connectedTcp) { Send.VerifyTcp(id); }
                            if (!connectedUdp) { Send.VerifyUdp(id); }
                            break;

                        case Mode.Web:
                            if (!connectedTcp) { Send.VerifyTcp(id); }
                            break;
                    }
                }

                void Success()
                {
                    verify.Stop();
                    Send.Connect(id);
                }

                void Timeout()
                {
                    if (Connected()) { Success(); return; }
                    Log.Write("Connection Timeout");
                    Disconnect();
                }

                bool Connected()
                {
                    return (mode == Mode.Default && connectedTcp && connectedUdp)
                           || (mode == Mode.Web && connectedTcp);
                }
            }

            public class Tcp
            {
                public TcpClient socket;

                private NetworkStream stream;
                private readonly int id;

                private Packet receiveData;
                private byte[] receiveBuffer;

                public Tcp(int id)
                {
                    this.id = id;
                }

                public void Start(TcpClient socket)
                {
                    this.socket = socket;

                    this.socket.ReceiveBufferSize = BufferSize;
                    this.socket.SendBufferSize = BufferSize;
                    this.socket.NoDelay = !TcpBatching;

                    stream = this.socket.GetStream();

                    receiveData = new Packet();
                    receiveBuffer = new byte[BufferSize];

                    stream.BeginRead(receiveBuffer, 0, BufferSize, Receive, null);

                    Server.Send.Register(id);
                }

                public void Stop()
                {
                    stream?.Close();
                    socket?.Close();

                    stream = null;
                    receiveData = null;
                    receiveBuffer = null;
                    socket = null;
                }

                internal void Send(Packet packet)
                {
                    try
                    {
                        if (socket == null) { return; }
                        if (!clients[id].connected && !IsConnectPacket(packet.ToArray())) { return; }
                        if (clients[id].suspended && IsDataPacket(packet.ToArray())) { return; }
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Error sending tcp data: {e}");
                    }
                }

                private void Receive(IAsyncResult result)
                {
                    try
                    {
                        int length = stream.EndRead(result);
                        if (length <= 0)
                        {
                            clients[id].Disconnect();
                            return;
                        }

                        byte[] data = new byte[length];
                        Array.Copy(receiveBuffer, data, length);
                        receiveData.Reset(Read(data));
                        stream.BeginRead(receiveBuffer, 0, BufferSize, Receive, null);
                    }
                    catch
                    {
                        clients[id].Disconnect();
                    }
                }

                private bool Read(byte[] packetData)
                {
                    int length = 0;

                    receiveData.SetBytes(packetData);

                    if (receiveData.LengthRemaining() >= 4)
                    {
                        length = receiveData.ReadInt();
                        if (length <= 0)
                        {
                            return true;
                        }
                    }

                    while (length > 0 && length <= receiveData.LengthRemaining())
                    {
                        byte[] data = receiveData.ReadBytes(length);
                        ThreadManager.QueueTick(() =>
                        {
                            using (Packet packet = new Packet(data))
                            {
                                int packetId = packet.ReadInt();
                                Server.Receive.packets[(Client.Client.Packets)packetId](id, packet);
                            }
                        });

                        length = 0;
                        if (receiveData.LengthRemaining() >= 4)
                        {
                            length = receiveData.ReadInt();
                            if (length <= 0)
                            {
                                return true;
                            }
                        }
                    }

                    if (length <= 1)
                    {
                        return true;
                    }

                    return false;
                }
            }

            public class Udp
            {
                public IPEndPoint ip;

                private int id;

                public Udp(int id)
                {
                    this.id = id;
                }

                public void Start(IPEndPoint ip)
                {
                    this.ip = ip;
                }

                public void Stop()
                {
                    ip = null;
                }

                internal void Send(Packet packet)
                {
                    try
                    {
                        if (ip == null) { return; }
                        if (!clients[id].connected && !IsConnectPacket(packet.ToArray())) { return; }
                        if (clients[id].suspended && IsDataPacket(packet.ToArray())) { return; }
                        udpListener.BeginSend(packet.ToArray(), packet.Length(), ip, null, null);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Error sending udp data: {e}");
                    }
                }

                public void Read(Packet dataPacket)
                {
                    int length = dataPacket.ReadInt();
                    byte[] data = dataPacket.ReadBytes(length);

                    ThreadManager.QueueTick(() =>
                    {
                        using (Packet packet = new Packet(data))
                        {
                            Receive.packets[(Client.Client.Packets)packet.ReadInt()](id, packet);
                        }
                    });
                }
            }
        }
    }
}