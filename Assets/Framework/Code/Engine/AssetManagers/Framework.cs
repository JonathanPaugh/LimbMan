using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Jape
{
    public class Framework : AssetManager<Framework>
    {
        public Framework() { Instance = this; }
        
        [NonSerialized]
        private List<SettingsData> cachedSettings = new();

        [SerializeField, HideInInspector]
        private FrameworkSettings settings;
        public static FrameworkSettings InternalSettings => Instance.settings != null ? Instance.settings : Instance.settings = Settings<FrameworkSettings>();

        public const string FrameworkDeveloperMode = "@Framework.DeveloperMode";

        public static bool DeveloperMode
        {
            get => Settings<EditorSettings>().DeveloperMode;
            set => Settings<EditorSettings>().DeveloperMode = value;
        }

        public static string ApplicationPath => IO.GetParentDirectory(Application.dataPath);
        public static string SystemPath => IO.JoinPath("System", "Resources");

        internal static bool TryGetSettings<T>(out T setting) where T : SettingsData
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

        [Pure]
        public static T Settings<T>() where T : SettingsData
        {
            if (!TryGetSettings(out T settings)) { Log.Write(SettingsWarning<T>()); return settings; } 
            return settings;
        }

        protected static string SettingsWarning<T>() where T : SettingsData => $"Could not get {typeof(T).CleanName()}, try creating an instance";

        public static bool InArchiveEditor(Object asset)
        {
            #if UNITY_EDITOR

            string path = UnityEditor.AssetDatabase.GetAssetPath(asset).ToLower();
            if (path.StartsWith(IO.JoinPath("Assets", "Archive"))) { return true; }

            #endif

            return false;
        }

        public static Sector? GetSectorEditor(Object asset)
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
                case Sector.Game: return InternalSettings.gamePath;
                case Sector.Framework: return InternalSettings.frameworkPath;
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
                    return IO.JoinPath(GetPath(sector), GetPath(region));

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
                    @namespace = InternalSettings.frameworkNamespace;
                    break;

                case Sector.Game:
                    @namespace = InternalSettings.gameNamespace;
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