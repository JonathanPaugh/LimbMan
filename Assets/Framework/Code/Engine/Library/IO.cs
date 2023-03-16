using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

using Object = UnityEngine.Object;

namespace Jape
{
    public static class IO
    {
        public const char DirectorySperator = '/';

        public static void CreateFile(string path)
        {
            File.Create(path).Close();
        }

        public static void WriteBytes(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        public static void WriteText(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        public static byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static string ReadText(string path)
        {
            return File.ReadAllText(path);
        }

        public static string GetParentDirectory(string path)
        {
            return ConvertPath(Directory.GetParent(path).FullName);
        }

        public static string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static IEnumerable<string> GetDirectoryFiles(string path)
        {
            return Directory.GetFiles(path).Select(ConvertPath);
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string JoinPath(IEnumerable<string> paths)
        {
            return string.Join(DirectorySperator, paths);
        }

        public static string JoinPath(string path, params string[] paths)
        {
            return JoinPath(new[] { path }.Concat(paths));
        }

        public static string ConvertPath(string path)
        {
            return path.Replace("\\", DirectorySperator.ToString());
        }

        public static void Delete(string path)
        {
            if (File.Exists(path)) { File.Delete(path); }
            if (File.Exists($"{path}.meta")) { File.Delete($"{path}.meta"); }
        }

        public static void Recycle(string path)
        {
            if (File.Exists(path)) { Shell.MoveToRecycleBin(path); }
            if (File.Exists($"{path}.meta")) { Shell.MoveToRecycleBin($"{path}.meta"); }
        }

        #if UNITY_EDITOR

        public static class Editor
        {
            public static string SelectionFolder
            {
                get
                {
                    Object selection = UnityEditor.Selection.activeObject;
                    return selection == null ? "Assets" : FindFolder(selection);
                }
            }

            public static string FileIndexName(string name, string path)
            {
                int index = GetFileIndex(name, path);
                if (index == 0) { return name; }
                return name + index;
            }

            public static int GetFileIndex(string name, string path)
            {
                int i = 0;
                while (true)
                {
                    string asset = name;
                    asset = i == 0 ? asset : $"{asset}{i}";
                    if (UnityEditor.AssetDatabase.FindAssets(asset, new [] { path }).Length == 0) { return i; }
                    i++;
                }
            }

            public static void CreateFileFolders(string path)
            {
                if (string.IsNullOrEmpty(path)) { Log.Write("Cannot create folders for empty path"); return; }

                List<string> folders = path.Split(DirectorySperator).
                    Where(f => !f.Contains(".")).
                    Where(f => !string.IsNullOrEmpty(f)).
                    ToList();

                for (int i = 0; i < folders.Count; i++)
                {
                    string folderPath = JoinPath(folders.Take(i + 1));
                    if (!Directory.Exists(folderPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder(JoinPath(folders.Take(i)), folders[i]);

                        // Unity will crash if created folder contains items before asset refresh
                        UnityEditor.AssetDatabase.Refresh();
                    }
                }
            }

            public static bool IsFile(Object item)
            {
                if (item == null) { Log.Write("Item is null"); return false; }

                string path = UnityEditor.AssetDatabase.GetAssetPath(item.GetInstanceID());

                if (!string.IsNullOrEmpty(path)) { return !Directory.Exists(path); }

                Log.Write($"{item} is not an asset");

                return false;
            }

            public static bool IsFolder(Object item)
            {
                if (item == null) { Log.Write("Item is null"); return false; }

                string path = UnityEditor.AssetDatabase.GetAssetPath(item.GetInstanceID());

                if (path.Length > 0)
                {
                    return Directory.Exists(path);
                }

                Log.Write($"{item} is not an asset");

                return false;
            }

            public static string FindFolder(Object item)
            {
                if (IsFolder(item)) { return UnityEditor.AssetDatabase.GetAssetPath(item); }
                if (IsFile(item))
                {
                    string folderPath = UnityEditor.AssetDatabase.GetAssetPath(item); 
                    return folderPath.Remove(folderPath.LastIndexOf(DirectorySperator));
                }

                Log.Write($"{item} folder cannot be found");

                return null;
            }

            public static void Delete(Object item)
            {
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(item));
            }

            public static void Recycle(Object item)
            {
                string filePath = JoinPath(Framework.ApplicationPath, UnityEditor.AssetDatabase.GetAssetPath(item));

                if (File.Exists(filePath)) { Shell.MoveToRecycleBin(filePath); }
                if (File.Exists($"{filePath}.meta")) { Shell.MoveToRecycleBin($"{filePath}.meta"); }
            }
        }

        #endif

        private static class Shell
        {
            [Flags]
            public enum OperationFlags : ushort
            {
                // ReSharper disable IdentifierTypo
                FOF_SILENT = 0x0004,
                FOF_NOCONFIRMATION = 0x0010,
                FOF_ALLOWUNDO = 0x0040,
                FOF_SIMPLEPROGRESS = 0x0100,
                FOF_NOERRORUI = 0x0400,
                FOF_WANTNUKEWARNING = 0x4000,
                // ReSharper restore IdentifierTypo
            }

            public enum OperationType : uint
            {
                FO_MOVE = 0x0001,
                FO_COPY = 0x0002,
                FO_DELETE = 0x0003,
                FO_RENAME = 0x0004,
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            private struct Operation
            {

                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.U4)]
                public OperationType wFunc;
                public string pFrom;
                public string pTo;
                public OperationFlags fFlags;
                [MarshalAs(UnmanagedType.Bool)]
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            private static extern int SHFileOperation(ref Operation FileOp);

            public static bool Send(string path, OperationFlags flags = OperationFlags.FOF_NOCONFIRMATION | OperationFlags.FOF_WANTNUKEWARNING)
            {
                try
                {
                    var fs = new Operation
                    {
                        wFunc = OperationType.FO_DELETE,
                        pFrom = path + '\0' + '\0',
                        fFlags = OperationFlags.FOF_ALLOWUNDO | flags
                    };
                    SHFileOperation(ref fs);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool MoveToRecycleBin(string path) { return Send(path, OperationFlags.FOF_NOCONFIRMATION | OperationFlags.FOF_NOERRORUI | OperationFlags.FOF_SILENT); }
        }
    }
}