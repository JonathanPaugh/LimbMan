using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class DataStream : Stream
    {
        private Queue<object> data;
        internal Queue<object> Data => data ?? (data = new Queue<object>());

        public int Count => Data.Count;

        public void SetData(object[] data)
        {
            if (data == null) { Data.Clear(); return; }
            this.data = new Queue<object>(data);
        }

        private void Read<T>(ref T value)
        {
            if (value.GetType() != Data.Peek().GetType())
            {
                this.Log().Warning("Reference and stored value types are not the same, skipping stream for this value"); 
                data.Dequeue(); 
                return;
            }
            value = (T)Data.Dequeue();
        }

        private void Write<T>(T value)
        {
            Data.Enqueue(value);
        }

        public override void Stop()
        {
            switch (mode)
            {
                case Mode.None:
                    Log.Write("Stream is not running"); 
                    return;

                case Mode.Reading:
                    if (Data.Count > 0) { this.Log().Warning("Stream did not finish reading all data"); }
                    break;
            }

            mode = Mode.None;
        }

        public void Stream<T>(ref T value)
        {
            switch (mode)
            {
                case Mode.None:
                    this.Log().Response("Stream is not running");
                    break;

                case Mode.Writing:
                    Write(value);
                    break;

                case Mode.Reading:
                    Read(ref value);
                    break;
            }
        }

        public object[] ToArray() { return Data.ToArray(); }
    }
}