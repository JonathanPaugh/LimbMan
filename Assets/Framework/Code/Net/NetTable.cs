using System;
using System.Collections.Generic;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public class NetTable
    {
        private Dictionary<int, Dictionary<string, Data>> tables = new Dictionary<int, Dictionary<string, Data>>();

        public object GetGlobal(string key) { return Get(0, key); }
        public void SetGlobal(string key, object value) { Set(0, key, value); }

        public object Get(int player, string key)
        {
            Dictionary<string, Data> table = GetTable(player);
            if (!table.TryGetValue(key, out Data value)) { return null; }
            return value.Value;
        }

        public void Set(int player, string key, object value)
        {
            Dictionary<string, Data> table = GetTable(player);
            if (!table.ContainsKey(key))
            {
                table.Add(key, new Data());
            }
            table[key].Value = value;
        }

        public void ListenStart(int client, int player, string key, int index)
        {
            Dictionary<string, Data> table = GetTable(player);
            if (!table.ContainsKey(key)) { table.Add(key, new Data()); }
            table[key].ListenStart(client, index);
        }

        public void ListenStop(int client, int player, string key)
        {
            Dictionary<string, Data> table = GetTable(player);
            if (!table.ContainsKey(key)) { return; }
            int index = table[key].ListenStop(player);
            if (index >= -1)
            {
                Server.Server.Send.ResponseClose(client, index);
            }
        }

        private Dictionary<string, Data> GetTable(int player)
        {
            if (!tables.TryGetValue(player, out Dictionary<string, Data> table))
            {
                tables.Add(player, new Dictionary<string, Data>());
                table = tables[player];
            }
            return table;
        }

        public class Data
        {
            private object value;
            public object Value
            {
                get => value;
                set
                {
                    if (this.value == value) { return; }
                    this.value = value;
                    foreach (KeyValuePair<int, int> listener in listeners)
                    {
                        NetMode mode = NetManager.GetMode();
                        if (mode.IsLocal)
                        {
                            Client.Client.netListener.Receive(listener.Value).Invoke(this.value);
                        } 
                        else
                        {
                            Server.Server.Send.Response(listener.Key, listener.Value, this.value, false);
                        }
                    }
                }
            }

            private readonly Dictionary<int, int> listeners = new Dictionary<int, int>();

            public void ListenStart(int client, int index)
            {
                if (!listeners.ContainsKey(client))
                {
                    listeners.Add(client, index);
                    return;
                }
                listeners[client] = index;
            }

            public int ListenStop(int client)
            {
                if (!listeners.ContainsKey(client)) { return -1; }
                int index = listeners[client];
                listeners.Remove(client);
                return index;
            }
        }
    }
}