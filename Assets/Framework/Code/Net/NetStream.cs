using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public class NetStream : Stream
    {
        private List<object> writeData;
        internal List<object> WriteData => writeData ?? (writeData = new List<object>());

        private List<Queue<object>> readData;
        internal List<Queue<object>> ReadData => readData ?? (readData = new List<Queue<object>>());

        internal Writer writer;
        internal Reader reader;

        public NetStream(int clientRate, int serverRate)
        {
            writer = new Writer(Write, IsWriting, clientRate, serverRate);
            reader = new Reader(Read, IsReading, HasNext);
        }

        internal bool CanSendData() { return writeData.Count > 0; }
        internal bool HasNext() { return ReadData.Count > 0; }

        internal void PushData(object[] data)
        {
            if (data == null) { return; }
            ReadData.Insert(0, new Queue<object>(data));
        }

        private void Write<T>(T value)
        {
            WriteData.Add(value);
        }

        private object[] Read()
        {
            object[] values = new object[ReadData.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = readData[i].Dequeue();
            }

            return values;
        }

        public void ClearWriteData() { WriteData.Clear(); }
        public void ClearReadData() { ReadData.Clear(); }

        internal object[] ToWriteDataArray() { return WriteData.ToArray(); }
        internal Queue<object>[] ToReadDataArray() { return ReadData.ToArray(); }

        public override void Stop()
        {
            switch (mode)
            {
                case Mode.None:
                    Log.Write("Stream is not running"); 
                    return;

                case Mode.Writing:
                    if (ReadData.Any(d => d.Count > 0)) { this.Log().Warning("Stream did not finish writing all data"); }
                    break;
            }

            mode = Mode.None;
        }

        public class Writer : ClientWriter, ServerWriter
        {
            private enum Mode { None, Client, Server }
            private Mode mode;

            private readonly Action<object> write;
            private readonly Func<bool> active;

            public int ClientRate { get; }
            public int ServerRate { get; }

            public int ClientStep { get; private set; }
            public int ServerStep { get; private set; }

            internal Writer(Action<object> write, Func<bool> active, int clientRate, int serverRate)
            {
                this.write = write;
                this.active = active;

                ClientRate = clientRate;
                ServerRate = serverRate;
            }

            private bool IsRunning() => mode != Mode.None;

            internal void StartClient()
            {
                if (IsRunning()) { this.Log().Response("Writer is already running"); return; }
                mode = Mode.Client;
            }

            internal void StartServer()
            {
                if (IsRunning()) { this.Log().Response("Writer is already running"); return; }
                mode = Mode.Server;
            }

            internal void Stop()
            {
                switch (mode)
                {
                    case Mode.None:
                        Log.Write("Writer is not running"); 
                        return;
                }

                mode = Mode.None;
            }

            public void Stream<T>(T value)
            {
                if (!active()) { Log.Write("Stream is not writing"); return; }
                if (!IsRunning()) { Log.Write("Writer is not running"); return; }
                Write(value);
            }

            private void Write(object value)
            {
                switch (mode)
                {
                    case Mode.Client:
                        if (ClientStep % ClientRate != 0) { return; }
                        break;

                    case Mode.Server:
                        if (ServerStep % ServerRate != 0) { return; }
                        break;
                }
                
                write(value);
            }

            internal void IncrementClient()
            {
                if (ClientStep >= ClientRate) { ClientStep = 1; return; }
                ClientStep++;
            }

            internal void IncrementServer()
            {
                if (ServerStep >= ServerRate) { ServerStep = 1; return; }
                ServerStep++;
            }
        }

        public interface IWriter
        {
            void Stream<T>(T value);
        }

        public interface ClientWriter : IWriter
        {
            int ClientRate { get; }
            int ClientStep { get; }
        }

        public interface ServerWriter : IWriter
        {
           int ServerRate { get; }
           int ServerStep { get; }
        }

        public class Reader : ClientReader, ServerReader
        {
            private readonly Func<object[]> read;
            private readonly Func<bool> active;
            private readonly Func<bool> hasNext;

            internal Reader(Func<object[]> read, Func<bool> active, Func<bool> hasNext)
            {
                this.read = read;
                this.active = active;
                this.hasNext = hasNext;
            }

            public T[] Stream<T>()
            {
                if (!active()) { Log.Write("Stream is not reading"); return default; }
                return read().Cast<T>().ToArray();
            }

            public T StreamFirst<T>()
            {
                return Stream<T>().FirstOrDefault();
            }

            public T StreamLast<T>()
            {
                return Stream<T>().LastOrDefault();
            }

            public bool HasNext() => hasNext();
        }

        public interface IReader
        {
            T[] Stream<T>();
            T StreamFirst<T>();
            T StreamLast<T>();
            bool HasNext();
        }
        public interface ClientReader : IReader {} 
        public interface ServerReader : IReader {}
    }
}