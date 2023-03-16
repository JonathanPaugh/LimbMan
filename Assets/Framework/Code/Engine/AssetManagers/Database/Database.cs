using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Jape
{
    public partial class Database : AssetManager<Database>, ISerializationCallbackReceiver
    {
        [Space(8)]

        [SerializeField, HideInInspector] 
        private Mode mode = Mode.Automatic;

        [Button(ButtonSizes.Large, DrawResult = false)]
        [LabelText(Selector.Indicator + nameof(mode))]
        public void ChangeMode()
        {
            switch (mode)
            {
                case Mode.Manual: mode = Mode.Automatic; break;
                case Mode.Automatic: mode = Mode.Manual; break;
            }
        }

        [PropertySpace(8)]

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Update Database")]
        #endif
        [Button(ButtonSizes.Large)]
        [HideIf(nameof(UpdateAutomatically))]
        internal static void UpdateEditor()
        {
            #if UNITY_EDITOR
            Instance.UpdateDatabase(true);
            Instance.OnAfterDeserialize();
            #endif
        }

        [PropertySpace(8)]

        [PropertyOrder(1)]
        [SerializeField]
        [InlineProperty]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [ListDrawerSettings(IsReadOnly = true, ShowPaging = false, Expanded = true)]
        [LabelText("Resources")]
        internal List<Resource> items = new();

        internal Resource root = new("", "", Resource.Type.Folder, "");

        [SerializeField, HideInInspector] 
        private int fileCount;
        public int FileCount => fileCount;

        [SerializeField, HideInInspector] 
        private int folderCount;
        public int FolderCount => folderCount;

        [HideInInspector] 
        public bool UpdateAutomatically => mode == Mode.Automatic;

        public enum Mode { Manual, Automatic }

        public Database() { Instance = this; }

        private void QuitEditor()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting -= QuitEditor;
            UpdateEditor();
            #endif
        }

        internal override void OnEnable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.quitting += QuitEditor;
            #endif

            base.OnEnable();
        }

        public static Resource GetFolder(string path)
        {
            return Instance.root.GetChild(path, Resource.Type.Folder);
        }

        public static IEnumerable<Resource> GetAssets(string name, Type assetType = null, bool strict = false)
        {
            return Instance.root.GetChildren(name, Resource.Type.Asset, true, assetType, strict);
        }

        public static IEnumerable<Resource> GetAssets<T>(string name = null, bool strict = false) where T : UnityEngine.Object
        {
            return GetAssets(name, typeof(T), strict);
        }

        public static Resource GetAsset(string name, Type assetType = null, bool strict = false)
        {
            return Instance.root.GetChildren(name, Resource.Type.Asset, true, assetType, strict).FirstOrDefault();
        }

        public static Resource GetAsset<T>(string name = null, bool strict = false) where T : UnityEngine.Object
        {
            return GetAsset(name, typeof(T), strict);
        }

        public static GameObject LoadPrefab(string name)
        {
            return GetAsset<GameObject>(name, true)?.Load<GameObject>();
        }

        private static void ScanFolder(DirectoryInfo folder, List<DirectoryInfo> list, bool onlyTopFolders)
        {
            string name = folder.Name.ToLower();

            switch (name)
            {
                case "editor":
                    return;

                case "resources":
                    list.Add(folder);
                    if (onlyTopFolders) { return; }
                    break;
            }

            foreach(var dir in folder.GetDirectories())
            {
                ScanFolder(dir, list, onlyTopFolders);
            }
        }

        private static List<DirectoryInfo> FindResourcesFolders(bool onlyTopFolders)
        {
            var assets = new DirectoryInfo(Application.dataPath);
            var list = new List<DirectoryInfo>();
            ScanFolder(assets, list, onlyTopFolders);
            return list;
        }

        private void AddFileList(DirectoryInfo folder, int prefix)
        {
            string relFolder = folder.FullName;
            relFolder = relFolder.Length < prefix ? "" : relFolder.Substring(prefix);

            relFolder = IO.ConvertPath(relFolder);
            foreach (var directoryFolders in folder.GetDirectories())
            {
                items.Add(new Resource(directoryFolders.Name, relFolder, Resource.Type.Folder, ""));
                AddFileList(directoryFolders, prefix);
            }

            foreach (var file in folder.GetFiles())
            {
                string ext = file.Extension.ToLower();

                if (ext == ".meta") { continue; }

                string assetPath = IO.JoinPath("Assets", file.FullName.Substring(Application.dataPath.Length + 1));
                assetPath = IO.ConvertPath(assetPath);

                Object obj = null;

                #if UNITY_EDITOR
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                #endif

                if (obj == null)
                {
                    this.Log().Response($"File at path {assetPath} couldn't be loaded and is ignored");
                    continue;
                }

                string type = obj.GetType().AssemblyQualifiedName;
                items.Add(new Resource(file.Name, relFolder, Resource.Type.Asset,type));
            }

            Resources.UnloadUnusedAssets();
        }

        internal void UpdateDatabase(bool save = false)
        {
            items.Clear();
            root.children.Clear();
            var topFolders = FindResourcesFolders(true);
            
            foreach(var folder in topFolders)
            {
                string path = folder.FullName;
                int prefix = path.Length;

                if (!path.EndsWith(IO.DirectorySperator)) { prefix++; }

                AddFileList(folder, prefix);
            }

            folderCount = 0;
            fileCount = 0;

            foreach (var item in items)
            {
                switch (item.ResourceType)
                {
                    case Resource.Type.Folder:
                        folderCount++;
                        break;
                    case Resource.Type.Asset:
                        fileCount++;
                        break;
                }
            }

            if (save)
            {
                SaveEditor();
            }
        }

        public new void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            #if UNITY_EDITOR
            if (items == null || items.Count == 0)
            {
                UpdateDatabase();
            }
            #endif
        }

        public new void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            root.children.Clear();
            foreach (var item in items)
            {
                item.OnDeserialize();
            }
        }
    }

    [Serializable]
    public class Resource : ISearchFilterable
    {
        public enum Type
        {
            Unknown = 0,
            Any = 0,
            Folder = 1,
            Asset = 2,
        }

        [FoldoutGroup("$" + nameof(name), false)]

        [SerializeField, HideInInspector]
        private string name;

        [FoldoutGroup("$" + nameof(name), false)]

        [SerializeField]
        [ReadOnly]
        private Type resourceType = Type.Unknown;

        [FoldoutGroup("$" + nameof(name), false)]

        [SerializeField]
        [ReadOnly]
        private string path;

        [FoldoutGroup("$" + nameof(name), false)]

        [SerializeField]
        [HideIf(nameof(resourceType), Type.Folder)]
        [LabelText("File")]
        [ReadOnly]
        private string ext;

        [FoldoutGroup("$" + nameof(name), false)]

        [SerializeField]
        [HideIf(nameof(resourceType), Type.Folder)]
        [ReadOnly]
        private string type;

        private System.Type objectType;
        private Resource parent;
        internal Dictionary<string, Resource> children;

        public string Name => name;
        public string Ext => ext;
        public string FolderPath => path;
        public string AssetPath => string.IsNullOrEmpty(path) ? name : IO.JoinPath(path, name);
        public Type ResourceType => resourceType;

        // ReSharper disable ConvertToAutoPropertyWithPrivateSetter
        public Resource Parent { get { return parent; } }
        // ReSharper restore ConvertToAutoPropertyWithPrivateSetter

        public Resource()
        {
            if (resourceType == Type.Folder) { children = new Dictionary<string, Resource>(); }
        }

        public Resource(string fileName, string path, Type type, string objectType)
        {
            var index = fileName.LastIndexOf('.');
            if (index > 0)
            {
                name = fileName.Substring(0, index);
                ext = fileName.Substring(index + 1);
            }
            else
            {
                name = fileName;
                ext = "";
            }
            this.path = path;
            resourceType = type;
            this.type = objectType;
            this.objectType = System.Type.GetType(this.type);

            if (resourceType == Type.Folder) { children = new Dictionary<string, Resource>(); }
        }

        public Resource GetChild(string path, Type resourceType = Type.Any)
        {
            if (this.resourceType != Type.Folder) { return null; }

            string p = path;
            int index = path.IndexOf('/');
            if (index > 0)
            {
                p = path.Substring(0, index);
                path = path.Substring(index + 1);
            }
            else
            {
                path = "";
            }

            if (!children.TryGetValue(p, out Resource item) || item == null) { return null; }

            if (path.Length > 0) { return item.GetChild(path, resourceType); }

            if (resourceType != Type.Unknown && item.resourceType != resourceType) { return null; }

            return item;
        }

        public IEnumerable<Resource> GetChildren(string name, Type resourceType = Type.Any, bool searchSubFolders = false, System.Type assetType = null, bool strict = false)
        {
            if (this.resourceType == Type.Asset) { yield break; }
            
            bool checkName = !string.IsNullOrEmpty(name);
            bool checkType = assetType != null;

            var items = children.Values;

            foreach (Resource item in items)
            {
                if (resourceType != Type.Any && resourceType != item.resourceType) { continue; }
                if (checkName && name != item.Name) { continue; }

                if (checkType)
                {
                    if (strict && assetType != item.objectType) { continue; }
                    if (!strict && !item.objectType.IsBaseOrSubclassOf(assetType)) { continue; }
                }

                yield return item;
            }

            if (searchSubFolders)
            {
                foreach (Resource item in items.Where(i => i.resourceType == Type.Folder).SelectMany(f => f.GetChildren(name, resourceType, searchSubFolders, assetType, strict)))
                {
                    yield return item;
                }
            }
        }

        public T[] LoadAll<T>() where T : UnityEngine.Object { return Resources.LoadAll<T>(AssetPath); }
        public UnityEngine.Object[] LoadAll() { return Resources.LoadAll(AssetPath); }
        public T Load<T>() where T : UnityEngine.Object { return Resources.Load<T>(AssetPath); }
        public UnityEngine.Object Load() { return Resources.Load(AssetPath); }

        internal void OnDeserialize()
        {
            parent = string.IsNullOrEmpty(path) ? Database.Instance.root : Database.GetFolder(path);

            if (parent != null)
            {
                if (!parent.children.ContainsKey(name))
                {
                    parent.children.Add(name, this);
                }
            }

            if (resourceType == Type.Folder)
            {
                children = new Dictionary<string, Resource>();
            }

            objectType = System.Type.GetType(type);
        }

        public bool IsMatch(string searchString)
        {
            return string.Equals(Name, searchString, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}