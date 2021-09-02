using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace Jape
{
    public abstract class DataType : Scriptable, Element.IReceivable
    {
        protected virtual bool Hidden => false;

        protected virtual void Created() {}

        private void OnCreate() { Created(); }

        internal void Save()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        internal void DestroyAsset()
        {
            #if UNITY_EDITOR
            Directory.Recycle(this);
            DestroyImmediate(this, true);
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        internal Sector CurrentSector()
        {
            Sector? sector = Framework.GetSector(this);
            if (sector == null) { throw new Exception("Sector not found"); }
            return (Sector)sector;
        }

        internal static T CreateData<T>(string path = null, string name = null) where T : DataType { return (T)CreateData(typeof(T), path, name); }

        internal static DataType CreateData(Type type, string path = null, string name = null)
        {
            DataType data = null;

            #if UNITY_EDITOR

            if (string.IsNullOrEmpty(name)) { name = type.ToString().RemoveNamespace(); }
            if (string.IsNullOrEmpty(path)) { path = Directory.SelectionFolder(); }

            Directory.CreateFileFolders(path);

            string file = $"{path}/{Directory.FileIndexName(name, path)}.asset";

            data = (DataType)CreateInstance(type);

            UnityEditor.AssetDatabase.CreateAsset(data, file);

            data.OnCreate();

            #endif

            return data;
        }

        [Pure] public static T Find<T>(string name) where T : DataType { return (T)Find(typeof(T), name); }
        [Pure] public static DataType Find(Type type, string name) { return Database.GetAsset(name, type)?.Load<DataType>(); }

        [Pure] public new static T[] FindAll<T>() where T : DataType { return FindAll(typeof(T)).Cast<T>().ToArray(); }
        [Pure] public new static IEnumerable<DataType> FindAll(Type type) { return Database.GetAssets(null, type).SelectMany(r => r.LoadAll<DataType>()); }

        public static IList<ValueDropdownItem<T>> Dropdown<T>() where T : DataType { return FindAll<T>().Select(d => new ValueDropdownItem<T>(d.name, d)).ToArray(); }
        public static IList<ValueDropdownItem<T>> DropdownStrict<T>() where T : DataType { return FindAll<T>().Where(d => d.GetType() == typeof(T)).Select(d => new ValueDropdownItem<T>(d.name, d)).ToArray(); }
    }
}