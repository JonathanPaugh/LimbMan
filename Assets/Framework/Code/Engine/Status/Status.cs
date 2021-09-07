using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
	public class Status : ISaveable
    {
        public string Key { get; set; }
        public byte[] Data => Serialize();

        private Dictionary<string, object> data = new Dictionary<string, object>();
        private object[] streamData;

        private DataStream stream;
        internal DataStream Stream => stream ?? (stream = new DataStream());

        /// <summary>
        /// Called before status is pushed to SaveManager
        /// </summary>
        protected virtual void OnSave() {}

        /// <summary>
        /// Called after status is pulled from SaveManager
        /// </summary>
        protected virtual void OnLoad() {}

        public bool Has(string key)
        {
            return data.ContainsKey(key);
        }

        public virtual object Read(string key)
        {
            if (!data.TryGetValue(key, out object value)) { return default; }
            return value;
        }

        public virtual void Write(string key, object value)
        {
            if (data.ContainsKey(key)) { data[key] = value; }
            else { data.Add(key, value); }
        }

        public virtual void Delete(string key)
        {
            if (!data.ContainsKey(key)) { return; }
            data.Remove(key);
        }

        public void StreamRead(Action<DataStream> stream)
        {
            Stream.StartReading();
            stream(Stream);
            Stream.Stop();
        }

        public void StreamWrite(Action<DataStream> stream)
        {
            Stream.StartWriting();
            stream(Stream);
            Stream.Stop();
        }

        public static void Save<T>(T status) where T : Status, new()
        {
            status.streamData = status.Stream.ToArray();
            status.OnSave();
            SaveManager.PushStatus(status.Key, status);
        }

        public static T Load<T>(string key) where T : Status, new()
        {
            if (!SaveManager.PullStatus(key, out T status)) { return null; }
            status.Stream.SetData(status.streamData);
            status.OnLoad();
            return status;
        }

        public virtual byte[] Serialize()
        {
            return Serializer.Bytes.Combine
            (
                Serializer.Dynamic.WriteDictionary(data), 
                Serializer.Dynamic.WriteList(streamData)
            );
        }

        public virtual int Deserialize(byte[] data)
        {
            int position = 0;

            position = Serializer.Dynamic.ReadDictionary(data, out Dictionary<string, object> dictionary, position);
            this.data = dictionary;

            position = Serializer.Dynamic.ReadList(data, out object[] list, position);
            streamData = list;

            return position;
        }
    }
}