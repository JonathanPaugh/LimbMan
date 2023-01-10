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

        public NetStream()
        {
            writer = new Writer(Write, IsWriting);
            reader = new Reader(Read, IsReading, HasNext);
        }

        internal bool CanSendData() { return WriteData.Count > 0; }
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

                case Mode.Reading:
                    if (ReadData.Any(d => d.Count > 0)) { this.Log().Warning("Stream did not finish reading all data"); }
                    break;
            }

            mode = Mode.None;
        }

        public class Writer : ClientWriter, ServerWriter
        {
            private readonly Action<object> write;
            private readonly Func<bool> active;

            internal Writer(Action<object> write, Func<bool> active)
            {
                this.write = write;
                this.active = active;
            }

            public void Stream<T>(T value)
            {
                if (!active()) { Log.Write("Stream is not writing"); return; }
                write(value);
            }
        }

        public interface IWriter
        {
            void Stream<T>(T value);
        }

        public interface ClientWriter : IWriter {} 
        public interface ServerWriter : IWriter {}

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