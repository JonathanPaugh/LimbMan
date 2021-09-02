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

        public void AutoSave()
        {
            if (Settings.autoSave)
            {
                Timer.CreateGlobal()
                     .Set(Settings.interval)
                     .ChangeMode(Timer.Mode.Loop)
                     .IterationAction(SaveAction)
                     .Start();
            }

            void SaveAction()
            {
                if (Settings.light) { SaveLight(); return; }
                Save();
            }
        }

        /// <summary>
        /// Save to file without requesting data
        /// </summary>
        internal static void Save()
        {
            if (!Settings.save) { return; }
            Instance.OnSaveRequest();
            SaveLight();
        }

        /// <summary>
        /// Save to file without requesting data
        /// </summary>
        internal static void SaveLight()
        {
            if (!Settings.save) { return; }
            Write();
        }

        /// <summary>
        /// Load from file
        /// </summary>
        internal static void Load(Action<bool> onLoad)
        {
            if (!Settings.load) { onLoad?.Invoke(false); return; }
            Read(onLoad);
        }

        public static void Delete()
        {
            if (!File.Exists(GetFile())) { Log.Write("Unable to find delete file"); return; }
            File.Delete(GetFile());
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

        private static void Write()
        {
            byte[] data = Serializer.Dynamic.WriteDictionary(Instance.data);
            switch (Game.IsWeb)
            {
                case true:
                {
                    WebManager.Save.RequestSave(Settings.Profile, data, null); 
                    break;
                }
                case false:
                {
                    File.WriteAllBytes(GetFile(), data); 
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
    }
}