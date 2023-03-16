using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace JapeEditor
{
    public class PickerWindow : Window
    {
        public enum Mode { Default, AssetsOnly, SceneOnly }

        [Flags]
        public enum ViewMode { Game = 1, Framework = 2, Archive = 4 }

        protected override string Title => "Picker";

        protected override Display DisplayMode => Display.Popup;
        
        protected override float Width => 1024;
        protected override float MinHeight => 512;

        private Type type;

        private Action<Object> action;

        private Mode mode;

        private List<Func<Object, bool>> filters = new();

        [ShowInInspector]
        [HideLabel]
        [OnValueChanged(nameof(RefreshItems))]
        [Range(5, 20)]
        private int columns = 10;

        [ShowInInspector]
        [HideLabel]
        [OnValueChanged(nameof(RefreshItems))]
        [EnumToggleButtons]
        private ViewMode view = Framework.DeveloperMode ? ViewMode.Game | ViewMode.Framework : ViewMode.Game;

        [ShowInInspector]
        [HideLabel]
        [OnValueChanged(nameof(Search))]
        private string search;

        private void Search() { RefreshItems(); }
        
        [ShowInInspector]
        [HideIf(nameof(IsEmpty))]
        [TableMatrix(DrawElementMethod = "DrawItem", HideColumnIndices = true, HideRowIndices = true, SquareCells = true, ResizableColumns = false)]
        private Item[,] items = new Item[0,0];

        private bool IsEmpty() { return items.Length == 0; }

        public void AddFilter(Func<Object, bool> filter)
        {
            filters.Add(filter);
        }

        public static PickerWindow Call(Type type, Action<Object> action, Mode mode = Mode.Default)
        {
            PickerWindow window = Open<PickerWindow>();
            
            window.type = type;
            window.action = action;
            window.mode = mode;

            return window;
        }

        internal void RefreshItems()
        {
            List<Item> @base = new();

            if (mode != Mode.AssetsOnly)
            {
                List<Item> scene = new();

                if (type == typeof(GameObject))
                {
                    scene.AddRange(FindObjectsOfType<GameObject>().Select(o => new Item(o, Item.Mode.Scene)));
                }

                if (type.IsBaseOrSubclassOf(typeof(Component)))
                {
                    Component[][] components = SceneManager.
                                               GetActiveScene().
                                               GetRootGameObjects().
                                               Select(g => g.GetComponentsInChildren(type)).
                                               ToArray();

                    foreach (Component[] componentArray in components)
                    {
                        scene.AddRange(componentArray.Select(o => new Item(o, Item.Mode.Scene)));
                    }
                }

                @base.AddRange(scene.OrderBy(i => i.Name));
            }

            if (mode != Mode.SceneOnly)
            {
                List<Item> assets = new();

                if (type == typeof(TextAsset))
                {
                    assets.AddRange(Jape.Game.FindDeep(typeof(MonoScript)).Select(o => new Item(o, Item.Mode.Asset)));
                }

                if (type.IsBaseOrSubclassOf(typeof(Component)))
                {
                    Component[][] components = Database.
                                               GetAssets<GameObject>().
                                               Select(r => r.Load<GameObject>()).
                                               Select(g => g.GetComponentsInChildren(type)).
                                               ToArray();

                    foreach (Component[] componentArray in components) { assets.AddRange(componentArray.Select(o => new Item(o, Item.Mode.Asset))); }
                }

                assets.AddRange(Database.GetAssets(null, type).
                       Select(r => r.Load()).
                       Select(o => new Item(o, Item.Mode.Asset)));

                @base.AddRange(assets.Where(AssetFilter).OrderBy(i => i.Name));
            }

            @base = filters.Aggregate(@base, (c, f) => c.Where(i => f(i.Value)).ToList());

            items = @base.
                    Where(Validate).
                    ToMatrixColumns(columns);
        }

        private bool Validate(Item item)
        {
            if (!string.IsNullOrEmpty(search)) { return item.Name.ToLowerInvariant().Contains(search.ToLowerInvariant()); }
            return true;
        }

        private bool AssetFilter(Item item)
        {
            if (item.Value == null) { return false; }
            if (item.Value.GetType() == typeof(MonoImporter)) { return false; }
            if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(item.Value))) { return false; }
            if (Framework.InArchiveEditor(item.Value)) { return view.HasFlag(ViewMode.Archive); }
            if (Framework.GetSectorEditor(item.Value) == Sector.Game) { return view.HasFlag(ViewMode.Game); }
            if (Framework.GetSectorEditor(item.Value) == Sector.Framework) { return view.HasFlag(ViewMode.Framework); }

            return false;
        }

        public Item DrawItem(Rect position, Item item)
        {
            GUIContent content = EditorGUIUtility.ObjectContent(item.Value, item.Type);
            content.text = null;
            
            EditorGUI.LabelField(position, content, new GUIStyle().Alignment(TextAnchor.MiddleCenter).Padding(new RectOffset(16, 16, 16, 16)));
            EditorGUI.LabelField(position, item.Name, new GUIStyle(GUI.skin.label).FontStyle(FontStyle.Bold).Alignment(TextAnchor.LowerCenter));

            GUIContent icon = new(item.GetIcon());
            EditorGUI.LabelField(new Rect(position.position, new Vector2(20, 20)), icon, new GUIStyle().Padding(new RectOffset(2, 0, 2, 0)));

            if (GUI.Button(position, GUIContent.none, GUIStyle.none))
            {
                action?.Invoke(item.Value);
            }

            return item;
        }

        [Serializable]
        public struct Item
        {
            public enum Mode { None, Scene, Asset }

            public Object Value { get; }

            private string name;
            public string Name
            {
                get
                {
                    if (Value == null) { return null; }
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public Type Type
            {
                get
                {
                    if (Value == null) { return null; }
                    return Value.GetType();
                }
            }

            public readonly Cache<GUIContent> icon;

            public GUIContent GetIcon() => icon.Value;

            public Item(Object value, Mode mode, string name = null)
            {
                Value = value;
                this.name = string.IsNullOrEmpty(name) ? value.name : name;
                string iconResource = mode switch
                {
                    Mode.Asset => "IconFolder",
                    Mode.Scene => "IconUnity",
                    _ => "IconQuestion",
                };
                icon = Cache<GUIContent>.CreateEditorManaged(() =>
                {
                    return new GUIContent(Database.GetAsset<Texture2D>(iconResource).Load<Texture2D>());
                });
            }
        }
    }
}