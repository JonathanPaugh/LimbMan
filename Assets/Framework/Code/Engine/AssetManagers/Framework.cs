using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Jape
{
    public class Framework : AssetManager<Framework>
    {
        public Framework() { Instance = this; }
        
        [NonSerialized]
        private List<SettingsData> cachedSettings = new List<SettingsData>();

        [SerializeField, HideInInspector]
        private FrameworkSettings settings;
        public static FrameworkSettings Settings => Instance.settings ?? (Instance.settings = Game.Settings<FrameworkSettings>());

        public static bool TryGetSetting<T>(out T setting) where T : SettingsData
        {
            setting = (T)Instance.cachedSettings.FirstOrDefault(s => s.GetType() == typeof(T));
            if (setting != null) { return true; }
            setting = Database.GetAsset<T>()?.Load<T>();
            if (setting != null)
            {
                Instance.cachedSettings.Add(setting);
                return true;
            }
            return false;
        }

        public static bool InArchive(Object asset)
        {
            #if UNITY_EDITOR

            string path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            if (path.StartsWith("Assets/Archive")) { return true; }

            #endif

            return false;
        }

        public static Sector? GetSector(Object asset)
        {
            #if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            if (path.StartsWith(GetPath(Sector.Framework))) { return Sector.Framework; }
            if (path.StartsWith(GetPath(Sector.Game))) { return Sector.Game; }
            #endif

            return null;
        }

        public static string GetPath(Sector sector)
        {
            switch (sector)
            {
                case Sector.Game: return Settings.gamePath;
                case Sector.Framework: return Settings.frameworkPath;
                default: return null;
            }
        }

        public static string GetPath(CodeRegion region)
        {
            switch (region)
            {
                case CodeRegion.Engine: return FrameworkSettings.EnginePath;
                case CodeRegion.Editor: return FrameworkSettings.EditorPath;
                case CodeRegion.Net: return FrameworkSettings.NetPath;
                default: return null;
            }
        }

        public static string GetPath(Sector sector, CodeRegion region)
        {
            switch (region)
            {
                case CodeRegion.Engine: case CodeRegion.Editor: case CodeRegion.Net:
                    return $"{GetPath(sector)}/{GetPath(region)}";

                default: 
                    return null;
            }
        }

        public static string GetNamespace(Sector sector, CodeRegion region)
        {
            string @namespace = string.Empty;

            switch (sector)
            {
                case Sector.Framework:
                    @namespace = Settings.frameworkNamespace;
                    break;

                case Sector.Game:
                    @namespace = Settings.gameNamespace;
                    break;
            }

            switch (region)
            {
                case CodeRegion.Engine:
                    @namespace += FrameworkSettings.EngineSuffix;
                    break;

                case CodeRegion.Editor:
                    @namespace += FrameworkSettings.EditorSuffix;
                    break;

                case CodeRegion.Net:
                    @namespace += FrameworkSettings.NetSuffix;
                    break;
            }

            return @namespace;
        }
    }
}