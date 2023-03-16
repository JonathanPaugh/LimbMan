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

        internal void DeleteEditor()
        {
            #if UNITY_EDITOR
            IO.Editor.Recycle(this);
            DestroyImmediate(this, true);
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        internal Sector CurrentSector()
        {
            Sector? sector = Framework.GetSectorEditor(this);
            if (sector == null) { throw new Exception("Sector not found"); }
            return (Sector)sector;
        }

        internal static T CreateData<T>(string path = null, string name = null, bool inspect = false) where T : DataType => (T)CreateData(typeof(T), path, name, inspect);

        internal static DataType CreateData(Type type, string path = null, string name = null, bool inspect = false)
        {
            #if UNITY_EDITOR

            if (string.IsNullOrEmpty(name)) { name = type.ToString().RemoveNamespace(); }
            if (string.IsNullOrEmpty(path)) { path = IO.Editor.SelectionFolder; }

            IO.Editor.CreateFileFolders(path);

            string file =  $"{IO.JoinPath(path, IO.Editor.FileIndexName(name, path))}.asset";

            DataType data = (DataType)CreateInstance(type);

            if (inspect)
            {
                UnityEditor.ProjectWindowUtil.ShowCreatedAsset(data);
            }

            UnityEditor.AssetDatabase.CreateAsset(data, file);
            UnityEditor.AssetDatabase.Refresh();

            data.OnCreate();

            return data;

            #else

            return null;

            #endif
        }

        [Pure] public static T Find<T>(string name) where T : DataType { return (T)Find(typeof(T), name); }
        [Pure] public static DataType Find(Type type, string name) { return Database.GetAsset(name, type)?.Load<DataType>(); }

        [Pure] public new static T[] FindAll<T>() where T : DataType { return FindAll(typeof(T)).Cast<T>().ToArray(); }
        [Pure] public new static IEnumerable<DataType> FindAll(Type type) { return Database.GetAssets(null, type).SelectMany(r => r.LoadAll<DataType>()); }

        public static IList<ValueDropdownItem<T>> Dropdown<T>() where T : DataType { return FindAll<T>().Select(d => new ValueDropdownItem<T>(d.name, d)).ToArray(); }
        public static IList<ValueDropdownItem<T>> DropdownStrict<T>() where T : DataType { return FindAll<T>().Where(d => d.GetType() == typeof(T)).Select(d => new ValueDropdownItem<T>(d.name, d)).ToArray(); }
    }
}