using System;
using UnityEngine;

namespace Jape
{
    public abstract class SystemData : DataType
    {
        protected static string Path => "System/Resources";

        internal static string GetPath<T>(Sector sector) where T : SystemData { return GetPath(typeof(T), sector); }

        internal static string GetPath(Type type, Sector sector)
        {
            string path = (string)Member.StaticDeep(type.Assembly, type.Name, nameof(Path)).Get();
            string basePath = sector == Sector.Game ? Framework.Settings.gamePath : Framework.Settings.frameworkPath;
            string fullPath = $"{basePath}/{path}";
            return fullPath;
        }
    }
}