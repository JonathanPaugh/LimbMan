using System;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public abstract class SystemData : DataType
    {
        protected static string SystemPath => Framework.SystemPath;
        protected static string Path => SystemPath;

        internal static string GetPath<T>(Sector sector) where T : SystemData { return GetPath(typeof(T), sector); }

        internal static string GetPath(Type type, Sector sector)
        {
            string path = (string)Member.StaticDeep(type.Assembly, type.Name, nameof(Path)).Get();
            string basePath = sector == Sector.Game ? Framework.InternalSettings.gamePath : Framework.InternalSettings.frameworkPath;
            string fullPath = IO.JoinPath(basePath, path);
            return fullPath;
        }
    }
}