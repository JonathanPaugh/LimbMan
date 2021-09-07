using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public sealed class SaveManager : Manager<SaveManager> 
    {
        private const string FileName = "";
        private const string Extension = "save";

        private new static bool InitOnLoad => true;

        private Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();

        internal Action OnSaveRequest = delegate {};
        internal Action OnLoadResponse = delegate {};

        private static SaveSettings settings;
        public static SaveSettings Settings => settings ?? (settings = Game.Settings<SaveSettings>());

        private static string GetFile() => $"{Application.persistentDataPath}/{FileName}{Settings.Profile}.{Extension}";

        public AutoSaver AutoSave { get; private set; }

        protected override void Init()
        {
            AutoSave = new AutoSaver();
        }

        /// <summary>
        /// Save to file without requesting data
        /// </summary>
        internal static void Save(Action onSave)
        {
            if (!Settings.save) { return; }
            Instance.OnSaveRequest();
            SaveLight(onSave);
        }

        /// <summary>
        /// Save to file without requesting data
        /// </summary>
        internal static void SaveLight(Action onSave)
        {
            if (!Settings.save) { return; }
            Write(onSave);
        }

        internal static void Load(Action<bool> onLoad)
        {
            if (!Settings.load) { onLoad?.Invoke(false); return; }
            Read(onLoad);
        }

        internal static void Delete(Action onDelete)
        {
            if (Game.IsRunning)
            {
                Instance.data.Clear();
            }
            switch (Game.IsWeb)
            {
                case true:
                    WebManager.Save.RequestDelete(Settings.Profile, onDelete);
                    break;

                case false:
                    File.Delete(GetFile());
                    onDelete?.Invoke();
                    break;
            }
        }

        public static void PushStatus<T>(string key, T status) where T : Status
        {
            if (Instance.data.ContainsKey(key)) { Instance.data[key] = status.Serialize(); }
            else { Instance.data.Add(key, status.Serialize()); }
        }

        public static bool PullStatus<T>(string key, out T status) where T : Status, new()
        {
            if (!Instance.data.TryGetValue(key, out byte[] bytes))
            {
                status = null;
                return false;
            }

            try
            {
                status = new T { Key = key };
                status.Deserialize(bytes);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unable to deserialize status");
                Debug.LogError(e);
                status = null;
                return false;
            }

            return true;
        }

        public static void RemoveStatus(string key)
        {
            if (!Instance.data.ContainsKey(key))
            {
                Debug.LogWarning("Unable to remove status");
                return;
            }

            Instance.data.Remove(key);
        }

        private static void Write(Action onWrite)
        {
            byte[] data = Serializer.Dynamic.WriteDictionary(Instance.data);
            switch (Game.IsWeb)
            {
                case true:
                {
                    WebManager.Save.RequestSave(Settings.Profile, data, onWrite); 
                    break;
                }
                case false:
                {
                    File.WriteAllBytes(GetFile(), data); 
                    onWrite?.Invoke();
                    break;
                }
            }
        }

        private static void Read(Action<bool> onRead)
        {
            switch (Game.IsWeb)
            {
                case true:
                {
                    WebManager.Save.RequestLoad(Settings.Profile, ReadData);
                    break;
                }
                case false:
                {
                    byte[] file;
                    try
                    {
                        file = File.ReadAllBytes(GetFile());
                    }
                    catch
                    {
                        Instance.Log().Diagnostic("Could not find file");
                        onRead?.Invoke(false);
                        return;
                    }
                    
                    ReadData(file);
                    break;
                }
            }

            void ReadData(byte[] data)
            {
                if (data.Length > 0)
                {
                    Serializer.Dynamic.ReadDictionary(data, out Dictionary<string, byte[]> dictionary);
                    Instance.data = dictionary;
                    Serializer.Dynamic.ShrinkAllocation();
                    onRead?.Invoke(true);
                    Instance.OnLoadResponse();
                }
                else
                {
                    onRead?.Invoke(false);
                }
            }
        }

        public class AutoSaver
        {
            private readonly Timer timer;

            private bool light;

            public AutoSaver()
            {
                timer = Timer.CreateGlobal().ChangeMode(Timer.Mode.Loop).IterationAction(Save);
            }

            private void Save()
            {
                switch (light)
                {
                    case true:
                        SaveManager.SaveLight(null);
                        break;

                    case false:
                        SaveManager.Save(null);
                        break;
                }
            }

            public void Start(float interval, bool light = false)
            {
                if (timer.IsProcessing())
                {
                    this.Log().Response("Unable to start, already processing");
                    return;
                }

                this.light = light;

                timer.Set(interval).Start();
            }

            public void Stop()
            {
                if (!timer.IsProcessing())
                {
                    this.Log().Response("Unable to stop, not processing");
                    return;
                }

                timer.Stop();
            }
        }
    }
}