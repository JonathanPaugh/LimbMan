using System;
using System.Net;
using System.Net.Sockets;
using Jape;

namespace JapeNet.Client
{
    public static partial class Client
    {
        public class Connection
        {
            internal const float Timeout = 10;

            internal enum Mode { Default, Web }
            internal Mode mode;

            public Tcp tcp;
            public Udp udp;
            public Web web;

            public int id;

            private bool running;

            internal bool connected;
            internal bool connectedTcp;
            internal bool connectedUdp;

            internal bool suspended;

            private int outPackets;
            private int outBytes;

            private int inPackets;
            private int inBytes;

            private Timer timeout;
            private Timer metrics;

            private Action success;
            private Action error;

            public Connection()
            {
                timeout = Timer.CreateGlobal();
                metrics = Timer.CreateGlobal().ChangeMode(Timer.Mode.Loop);

                if (Game.IsWeb)
                {
                    mode = Mode.Web;
                    web = new Web();
                }
                else
                {
                    mode = Mode.Default;
                    tcp = new Tcp();
                    udp = new Udp();
                }
            }

            public void Connect(Action success, Action error, int timeout)
            {
                if (running) { Log.Write("Client already running"); return; }
                running = true;

                this.success = success;
                this.error = error;

                try
                {
                    switch (mode)
                    {
                        case Mode.Default:
                            tcp.Start();
                            break;

                        case Mode.Web:
                            web.Start();
                            break;
                    }
                }
                catch
                {
                    error?.Invoke();
                    return;
                }

                if (timeout != -1)
                {
                    this.timeout.Set(timeout > 0 ? timeout : Connection.Timeout, Time.Counter.Realtime, Connection.Timeout)
                                .CompletedAction(Timeout)
                                .Start();
                }

                void Timeout()
                {
                    if (connected) { return; }
                    Log.Write("Connection Timeout");
                    this.error?.Invoke();
                    Disconnect();
                }
            }

            public void Disconnect()
            {
                if (!running) { Log.Write("Client not running"); return; }
                running = false;

                success = null;
                error = null;

                switch (mode)
                {
                    case Mode.Default:
                        tcp.Stop();
                        connectedTcp = false;

                        udp.Stop();
                        connectedUdp = false;
                        break;

                    case Mode.Web:
                        web.Stop();
                        connectedTcp = false;
                        break;
                }

                if (connected)
                {
                    connected = false;

                    StopMetrics();

                    Log.Write("Disconnected");

                    NetManager.DisconnectClient(id);
                }

                Log.Write("Client Stopped");

                NetManager.StopClient();
            }

            internal void Connected()
            {
                if (!running) { Log.Write("Client not running"); return; }

                timeout.Stop();

                success?.Invoke();

                StartMetrics();
            }

            internal void StartMetrics()
            {
                if (metrics.IsProcessing()) { return; }

                string spaceKey = MetricManager.Space();
                MetricManager.Set("Net", string.Empty);
                IO();
                SetLatencyMetrics(0);

                metrics.Set(1, Time.Counter.Realtime, 1)
                .ProcessedAction(() =>
                {
                    MetricManager.Remove(spaceKey);
                    MetricManager.Remove("Net");
                    MetricManager.Remove("Out");
                    MetricManager.Remove("In");
                    MetricManager.Remove("Latency");
                }).IntervalAction(() =>
                {
                    IO();
                    Ping();
                }).Start();

                void IO()
                {
                    MetricManager.Set("Out", $"({outPackets}) {outBytes / 1000} KB/s");
                    MetricManager.Set("In", $"({inPackets}) {inBytes / 1000} KB/s");

                    outPackets = 0;
                    outBytes = 0;

                    inPackets = 0;
                    inBytes = 0;
                }

                void Ping()
                {
                    Send.Ping();
                }
            }

            internal void SetLatencyMetrics(int latency)
            {
                MetricManager.Set("Latency", latency.ToString());
            }

            internal void StopMetrics()
            {
                if (!metrics.IsProcessing()) { return; }
                metrics.Stop();
            }

            public class Tcp
            {
                public TcpClient socket;

                private NetworkStream stream;

                private Packet receiveData;
                private byte[] receiveBuffer;

                public void Start()
                {
                    socket = new TcpClient
                    {
                        ReceiveBufferSize = BufferSize,
                        SendBufferSize = BufferSize,
                        NoDelay = !TcpBatching
                    };

                    receiveBuffer = new byte[BufferSize];
                    socket.BeginConnect(Ip, Port, Connect, socket);
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

                private void Connect(IAsyncResult result)
                {
                    socket.EndConnect(result);

                    if (!socket.Connected) { return; }

                    stream = socket.GetStream();

                    receiveData = new Packet();

                    stream.BeginRead(receiveBuffer, 0, BufferSize, Receive, null);
                }

                internal void Send(Packet packet)
                {
                    try
                    {
                        if (socket == null) { return; }
                        byte[] bytes = packet.ToArray();
                        if (server.suspended && IsDataPacket(bytes)) { return; }
                        server.outPackets += 1;
                        server.outBytes += bytes.Length;
                        stream.BeginWrite(bytes, 0, packet.Length(), null, null);
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
                            Client.Stop();
                            return;
                        }

                        byte[] data = new byte[length];
                        Array.Copy(receiveBuffer, data, length);

                        server.inBytes += length;
                        receiveData.Reset(Read(data));

                        stream.BeginRead(receiveBuffer, 0, BufferSize, Receive, null);
                    }
                    catch
                    {
                        Client.Stop();
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

                        server.inPackets += 1;
                        ThreadManager.QueueFrame(() =>
                        {
                            using (Packet packet = new Packet(data))
                            {
                                Client.Receive.packets[(Server.Server.Packets)packet.ReadInt()](packet);
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
                public UdpClient socket;

                public IPEndPoint ip;

                public void Start(int localPort)
                {
                    ip = new IPEndPoint(Ip, Port);
                    socket = new UdpClient(localPort);

                    socket.Connect(ip);
                    socket.BeginReceive(Receive, null);

                    using (Packet packet = new Packet())
                    {
                        Send(packet);
                    }
                }

                public void Stop()
                {
                    ip = null;
                    socket = null;
                }

                internal void Send(Packet packet)
                {
                    packet.InsertInt(server.id);
                    try
                    {
                        if (socket == null) { return; }
                        byte[] bytes = packet.ToArray();
                        if (server.suspended && IsDataPacket(bytes)) { return; }
                        server.outPackets += 1;
                        server.outBytes += bytes.Length;
                        socket.BeginSend(bytes, packet.Length(), null, null);
                    }
                    catch (Exception e)
                    {
                        Log.Write($"Error sending udp data: {e}");
                    }
                }

                private void Receive(IAsyncResult result)
                {
                    try
                    {
                        byte[] data = socket.EndReceive(result, ref ip);
                        socket.BeginReceive(Receive, null);

                        int length = data.Length;

                        if (length < 4)
                        {
                            Client.Stop();
                            return;
                        }

                        server.inBytes += length;
                        Read(data);
                    }
                    catch
                    {
                        Client.Stop();
                    }
                }

                private void Read(byte[] data)
                {
                    using (Packet packet = new Packet(data))
                    {
                        int length = packet.ReadInt();
                        data = packet.ReadBytes(length);
                    }

                    server.inPackets += 1;
                    ThreadManager.QueueFrame(() =>
                    {
                        using (Packet packet = new Packet(data))
                        {
                            int pack = packet.ReadInt();
                            Client.Receive.packets[(Server.Server.Packets)pack](packet);
                        }
                    });
                }
            }

            public class Web
            {
                private Packet receiveData;

                public void Start()
                {
                    WebManager.Socket.Connect(NetManager.Settings.ServerIp);
                    receiveData = new Packet();
                }

                public void Stop()
                {
                    WebManager.Socket.Disconnect();
                }

                internal void Send(Packet packet)
                {
                    byte[] data = packet.ToArray();

                    if (data.Length < 1) { return; }

                    server.outPackets += 1;
                    server.outBytes += data.Length;

                    NetManager.Instance.WebSocketSend(data);
                }

                internal void Receive(byte[] data)
                {
                    try
                    {
                        int length = data.Length;

                        if (length <= 0)
                        {
                            Client.Stop();
                            return;
                        }

                        server.inBytes += length;
                        receiveData.Reset(Read(data));
                    }
                    catch
                    {
                        Client.Stop();
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

                        server.inPackets += 1;
                        ThreadManager.QueueFrame(() =>
                        {
                            using (Packet packet = new Packet(data))
                            {
                                Client.Receive.packets[(Server.Server.Packets)packet.ReadInt()](packet);
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
        }
    }
}