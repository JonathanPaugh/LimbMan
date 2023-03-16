using System;
using System.Collections.Generic;
using System.Linq;
using Jape;

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

        public NetStream(int rate)
        {
            writer = new Writer(Write, IsWriting, rate);
            reader = new Reader(Read, IsReading, HasReadData);
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

        internal bool HasWriteData() { return WriteData.Count > 0; }
        internal bool HasReadData() { return ReadData.Count > 0; }

        public void ClearWriteData() { WriteData.Clear(); }
        public void ClearReadData() { ReadData.Clear(); }

        internal object[] PullData() { return WriteData.ToArray(); }
        internal void PushData(object[] data)
        {
            if (data == null) { return; }
            ReadData.Insert(0, new Queue<object>(data));
        }

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

        public void WriteNext(Action onWrite)
        {
            writer.Increment();
            StartWriting();
            onWrite();
            Stop();
        }

        public void ReadNext(Action onRead)
        {
            StartReading();
            onRead();
            Stop();
        }

        public class Writer : IClientWriter, IServerWriter
        {
            private readonly Action<object> write;
            private readonly Func<bool> active;

            public int Rate { get; }
            public int Step { get; private set; }

            internal Writer(Action<object> write, Func<bool> active, int rate)
            {
                this.write = write;
                this.active = active;
                Rate = rate;
            }

            public void Stream<T>(T value)
            {
                if (!active()) { Log.Write("Stream is not writing"); return; }
                Write(value);
            }

            private void Write(object value)
            {
                if (Step % Rate != 0) { return; }
                write(value);
            }

            internal void Increment()
            {
                if (Step >= Rate) { Step = 1; return; }
                Step++;
            }
        }

        public interface IWriter
        {
            void Stream<T>(T value);
        }

        public interface IClientWriter : IWriter {} 
        public interface IServerWriter : IWriter {}

        public class Reader : IClientReader, IServerReader
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

        public interface IClientReader : IReader {} 
        public interface IServerReader : IReader {}
    }
}