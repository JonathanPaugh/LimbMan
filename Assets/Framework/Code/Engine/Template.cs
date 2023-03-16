using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class Template
    {
        protected virtual string Extension => "txt";
        protected virtual bool ShowTerms => true;

        public Func<Sector> Sector { get; set; }
        public string Path { get; set; } = IO.JoinPath(Framework.SystemPath, "Templates");
        public string Content { get; set; } = string.Empty;

        [HorizontalGroup("Template", Width = 0.33f)]

        protected virtual string Name
        {
            get
            {
                if (IsSet())
                {
                    name = template.name; 
                    return name;
                }
                return name;
            }
            set { name = value; }
        }

        protected string name = string.Empty;

        [HorizontalGroup("Template")]

        [HideLabel]
        public TextAsset template;

        [HorizontalGroup("Button")]

        [DisableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        [LabelText("Create")]
        public virtual void CreateEditor()
        {
            #if UNITY_EDITOR

            InputWindow.Call(input =>
            {
                string path = IO.JoinPath(Framework.GetPath(GetSector()), Path);
                IO.Editor.CreateFileFolders(path);
                string file = IO.JoinPath(path, $"{IO.Editor.FileIndexName(input, path)}.{Extension}");

                FileWriter writer = new();
                writer.SetContent(Content);
                writer.WriteEditor(file);

                template = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(file);

            }, "Create Template", "Create", Name);
            #endif
        }

        [HorizontalGroup("Button")]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        [LabelText("Open")]
        public void OpenEditor()
        {
            #if UNITY_EDITOR
            if (template == null) { return; }
            UnityEditor.AssetDatabase.OpenAsset(template);
            #endif
        }

        [HorizontalGroup("Button")]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        [LabelText("Delete")]
        public void DeleteEditor()
        {
            #if UNITY_EDITOR
            if (template == null) { return; }
            IO.Editor.Recycle(template);
            UnityEditor.AssetDatabase.Refresh();
            name  = string.Empty;
            #endif
        }

        [PropertySpace(16)]

        [Title(nameof(Terms), titleAlignment: TitleAlignments.Centered)]

        [ShowInInspector]
        [PropertyOrder(1)]
        [HideLabel, ReadOnly]
        [ShowIf(nameof(ShowTerms))]
        [EnumToggleButtons]
        public FileWriter.Term Terms => 0;

        protected Sector GetSector() { return Sector?.Invoke() ?? Jape.Sector.Game; }

        private bool IsEmpty() { return string.IsNullOrEmpty(name); }
        public bool IsSet() { return template != null; }
    }
}