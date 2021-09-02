using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class Script : SystemData
    {
        public const string DefaultName = "Script";

        public enum Mode { Script, Behaviour, Modifier }

        protected virtual Mode GetMode() => Mode.Script;
        
        protected new static string Path => "System/Resources/Scripts";
        protected virtual string TemplatePath => "System/Resources/Templates/Scripts";

        protected virtual string PathSuffix => string.Empty;

        [SerializeField]
        [EnableIf(Game.GameDeveloperMode)]
        [ShowIf(nameof(GetMode), Mode.Script)]
        [HideInInlineEditors]
        [EnumToggleButtons]
        private SectorFlags sector = SectorFlags.Game;

        [Space(4)]

        [SerializeField]
        [HideInInlineEditors]
        [ShowIf(nameof(GetMode), Mode.Script)]
        [EnumToggleButtons]
        private CodeRegionFlags codeRegion = CodeRegionFlags.Engine;

        [Space(16)]

        [SerializeField]
        [ShowIf(nameof(GetMode), Mode.Script)]
        public bool hidden = false;

        [Space(32)]

        [SerializeField]
        [Partial(Selector.Indicator + nameof(DisplayPath))]
        [ShowIf(nameof(GetMode), Mode.Script)]
        public string path = string.Empty;

        [Space(4)]

        [ShowIf(nameof(GetMode), Mode.Script)]
        public string fileName = string.Empty;

        [Space(4)]

        [SerializeField]
        [ShowIf(nameof(GetMode), Mode.Script)]
        public string namespaceSuffix = string.Empty;

        [PropertySpace(32)]

        [Title("Template", TitleAlignment = TitleAlignments.Centered)]

        [OdinSerialize]
        [PropertyOrder(10)]
        [ShowInInspector]
        [HideLabel, HideReferenceObjectPicker, HideInInlineEditors]
        protected Template template = new Template();

        private Func<int, string> inject;
        public Func<int, string> Inject
        {
            get { return inject ?? (inject = BaseInject); }
            set { inject = value; }
        }

        protected string BaseInject(int index) { return index == 0 ? fileName : null; }

        public SectorFlags GetSectors() { return sector; }
        public CodeRegionFlags GetRegions() { return codeRegion; }

        public string GetFullPath(Sector sector, CodeRegion region)
        {
            return string.IsNullOrEmpty(path) ? GetPath(sector, region) : GetPath(sector, region) + "/" + path;
        }

        protected string GetPath(Sector sector, CodeRegion region)
        {
            return string.Join("/", new[] { Framework.GetPath(sector, region), PathSuffix }.Where(s => !string.IsNullOrEmpty(s)));
        }

        protected string GetPartialPath(Sector sector) { return Framework.GetPath(sector); }
        protected string GetPartialPath(CodeRegion codeRegion) { return Framework.GetPath(codeRegion); }

        protected string DisplayPath()
        {
            string path;

            int? sectorValue = sector.ConvertFromFlags();
            if (sectorValue.HasValue && ((int)sector).IsSingleBit()) { path = GetPartialPath((Sector)sectorValue.Value); }
            else { path = "..."; }

            int? regionValue = codeRegion.ConvertFromFlags();
            if (regionValue.HasValue && ((int)codeRegion).IsSingleBit()) { path = string.Join("/", path, GetPartialPath((CodeRegion)regionValue.Value)); }
            else { path = string.Join("/", path, "..."); }

            return string.Join("/", new[] { path, PathSuffix }.Where(s => !string.IsNullOrEmpty(s)).Append(string.Empty));
        }

        public string GetNamespace(Sector sector, CodeRegion region)
        {
            return string.IsNullOrEmpty(namespaceSuffix) ? 
                   Framework.GetNamespace(sector, region) ?? "" : 
                   $"{Framework.GetNamespace(sector, region)}.{namespaceSuffix}";
        }

        public static string[] GetDirectives(Sector sector, CodeRegion codeRegion)
        {
            List<string> directives = new List<string>();
            if (codeRegion == CodeRegion.Editor) { directives.Add("UnityEditor"); }
            if (sector == Sector.Game || codeRegion == CodeRegion.Editor) { directives.Add(Framework.Settings.frameworkNamespace); }
            if (sector == Sector.Game && codeRegion == CodeRegion.Editor) { directives.Add(Framework.Settings.frameworkNamespace + FrameworkSettings.EditorSuffix); }
            if (sector == Sector.Game && codeRegion == CodeRegion.Net) { directives.Add(Framework.Settings.frameworkNamespace + FrameworkSettings.NetSuffix); }
            return directives.ToArray();
        }

        protected override void EnabledEditor()
        {
            #if UNITY_EDITOR

            template.Sector = CurrentSector;
            template.Path = TemplatePath;

            #endif
        }

        internal void Create(Sector sector, CodeRegion region)
        {
            #if UNITY_EDITOR

            string name = string.IsNullOrWhiteSpace(fileName) ? DefaultName : fileName;

            string path = GetFullPath(sector, region);
            Directory.CreateFileFolders(path);

            string file = path + "/" + name;
            
            UnityEditor.AssetDatabase.CreateAsset(new TextAsset(), file);
            string resourceFile = path + "/" + UnityEditor.AssetDatabase.AssetPathToGUID(file);
            UnityEditor.AssetDatabase.RenameAsset(file, UnityEditor.AssetDatabase.AssetPathToGUID(file));

            UnityEditor.ProjectWindowUtil.StartNameEditingIfProjectWindowExists
            (
                0, 
                CreateInstance<Writer>().Set(template.template.text, GetNamespace(sector, region), GetDirectives(sector, region), Inject), 
                file + ".cs", 
                null, 
                resourceFile
            );

            Inject = null;

            #endif
        }

        internal TextAsset CreateAsset(Sector sector, CodeRegion region, string name)
        {
            #if UNITY_EDITOR

            string path = GetFullPath(sector, region);
            Directory.CreateFileFolders(path);

            string file = path + "/" + name;
            
            FileWriter fileWriter = new FileWriter();
            fileWriter.SetContent(template.template.text);

            fileWriter.SetNamespace(GetNamespace(sector, region));
            fileWriter.SetDirectives(GetDirectives(sector, region));
            fileWriter.SetInject(Inject);
            fileWriter.Write(file + ".cs");

            Inject = null;

            return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(file + ".cs");

            #endif

            #pragma warning disable 162
            return null;
            #pragma warning restore 162
        }

        public static IEnumerable<Script> GetScripts(Mode mode, Sector sector) 
        {
            switch (mode)
            {
                case Mode.Script:
                    return FindAll<Script>().
                           Where(s => !s.hidden).
                           Where(s => s.GetSectors().HasFlag((SectorFlags)sector.ConvertToFlags())).
                           ToArray();

                case Mode.Behaviour:
                    return FindAll<BehaviourType>().
                           Where(s => !s.hidden).
                           Where(s => s.GetSectors().HasFlag((SectorFlags)sector.ConvertToFlags())).
                           ToArray();

                default: return null;
            }
        }

        #if UNITY_EDITOR
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class Writer : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            private string content;
            private string @namespace;
            private string[] directives;
            private Func<int, string> inject;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                UnityEditor.AssetDatabase.DeleteAsset(resourceFile);
                FileWriter fileWriter = new FileWriter();
                fileWriter.SetContent(content);
                fileWriter.SetNamespace(@namespace);
                fileWriter.SetDirectives(directives);
                fileWriter.SetInject(inject);
                fileWriter.Write(pathName);
                UnityEditor.AssetDatabase.Refresh();
            }

            public Writer Set(string content, string @namespace, string[] directives, Func<int, string> inject)
            {
                this.content = content;
                this.@namespace = @namespace;
                this.directives = directives;
                this.inject = inject;
                return this;
            }
        }
        #endif

    }
}
