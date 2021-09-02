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
        public string Path { get; set; } = "System/Resources/Templates";
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
        public virtual void Create()
        {
            InputWindow.Call(Action, "Create Template", "Create", Name);

            void Action(string input)
            {
                #if UNITY_EDITOR
                string path = $"{Framework.GetPath(GetSector())}/{Path}";
                Directory.CreateFileFolders(path);
                string file = $"{path}/{Directory.FileIndexName(input, path)}.{Extension}";
                FileWriter writer = new FileWriter();
                writer.SetContent(Content);
                writer.Write(file);
                template = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                #endif
            }
        }

        [HorizontalGroup("Button")]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        public void Open()
        {
            #if UNITY_EDITOR
            if (template == null) { return; }
            UnityEditor.AssetDatabase.OpenAsset(template);
            #endif
        }

        [HorizontalGroup("Button")]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        public void Delete()
        {
            #if UNITY_EDITOR
            if (template == null) { return; }
            Directory.Recycle(template);
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