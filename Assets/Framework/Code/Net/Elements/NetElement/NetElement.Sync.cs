using System.Collections.Generic;
using System.Linq;

namespace JapeNet
{
    public abstract partial class NetElement
    {
        private class Sync
        {
            internal Dictionary<string, object> ReadData { get; set; } = new();
            private Dictionary<string, SyncValue> writeData = new();
            
            public Dictionary<string, object> Pull() => writeData.Where(d => d.Value.CanSend()).ToDictionary(d => d.Key, d => d.Value.Send());

            public bool Read(string key, out object value)
            {
                bool hasValue = ReadData.TryGetValue(key, out value);

                if (hasValue)
                {
                    ReadData.Remove(key);
                }

                return hasValue;
            }

            public void Write(string key, object value)
            {
                if (writeData.ContainsKey(key)) { writeData[key].Set(value); } 
                else { writeData.Add(key, new SyncValue(value)); }
            }

            public bool CanSend()
            {
                if (!writeData.Any()) { return false; }
                return writeData.Any(d => d.Value.CanSend());
            }

            private class SyncValue
            {
                private object value;
                private bool sent;

                public SyncValue(object value) { this.value = value; }
                public bool CanSend() { return !sent; }

                public void Set(object value)
                {
                    if (this.value.Equals(value)) { return; }
                    sent = false;
                    this.value = value;
                }

                public object Send()
                {
                    sent = true;
                    return value;
                }
            }
        }
    }
}