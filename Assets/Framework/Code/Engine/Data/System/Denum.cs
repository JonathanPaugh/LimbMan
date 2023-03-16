using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class Denum : SystemData
    {
        [Flags] public enum FlagsMode { Default = 1, Flags = 2 }

        public static CodeRegion CodeRegion = CodeRegion.Engine;

        protected new static string Path => IO.JoinPath(SystemPath, "Denums");
        private static string ScriptPath => "Denums";

        [ShowInInspector]
        [PropertyOrder(-1)]
        [EnableIf(Framework.FrameworkDeveloperMode)]
        [EnumToggleButtons]
        public Sector Sector
        {
            get => sector;
            set
            {
                if (sector == value) { return; }
                sector = value;
                DeleteScriptEditor();
                WriteEditor();
            }
        }

        [SerializeField, HideInInspector]
        private Sector sector = Sector.Game;

        [PropertySpace(8)]

        [ShowInInspector]
        [PropertyOrder(-1)]
        [EnumToggleButtons]
        public FlagsMode Mode
        {
            get => mode;
            set
            {
                if (mode == value) { return; }
                mode = value;
                WriteEditor();
            }
        }

        [SerializeField, HideInInspector]
        private FlagsMode mode = FlagsMode.Default | FlagsMode.Flags;

        [PropertySpace(8)]

        [ShowInInspector]
        [PropertyOrder(-1)]
        public bool UseNone
        {
            get => useNone;
            set
            {
                if (useNone == value) { return; }
                useNone = value;
                WriteEditor();
            }
        }

        [SerializeField, HideInInspector]
        private bool useNone;

        [Space(16)]

        [HideLabel, ReadOnly]
        public TextAsset script;

        [Space(8)]

        [ListDrawerSettings(ShowPaging = false)]
        public ObservableCollection<string> entries = new();

        private string storedName = string.Empty;

        internal static Script Template => Database.GetAsset<Script>(nameof(Denum)).Load<Script>();

        private bool IsDefault() { return Mode.HasFlag(FlagsMode.Default); }
        private bool IsFlags() { return Mode.HasFlag(FlagsMode.Flags); }

        private bool IsNewName() { return !string.IsNullOrEmpty(storedName) && storedName != name; }

        private string GetPath() { return Framework.GetPath(sector); }

        protected override void EnabledEditor()
        {
            entries.CollectionChanged += QueueRewrite;
        }

        protected override void DisabledEditor()
        {
            entries.CollectionChanged -= QueueRewrite;
        }

        private void QueueRewrite(object sender, NotifyCollectionChangedEventArgs e)
        {
            DenumManager.QueueRewriteEditor(this);
        }

        private void Change()
        {
            DeleteScriptEditor();
            storedName = name;
        }

        public void RewriteEditor()
        {
            #if UNITY_EDITOR
            if (entries == null) { return; }
            if (entries.Count == 0) { DeleteScriptEditor(); }
            WriteEditor();
            #endif
        }

        private void WriteEditor()
        {
            #if UNITY_EDITOR

            if (IsNewName()) { Change(); }

            Script template = Template;
            template.Inject = Inject;
            script = template.CreateAssetEditor(sector, CodeRegion, name);

            string Inject(int index)
            {
                const string FlagsAttribute = "[Flags]";

                switch (index) {
                    case 0:
                    {
                        if (!IsDefault()) { return string.Empty; }
                        return GetEnumString(name, entries.Select(e => $"{e} = {entries.IndexOf(e) + 1}"));
                    }

                    case 1:
                    {
                        if (!IsFlags()) { return string.Empty; }
                        return GetEnumString($"{name}Flags", entries.Select(e => $"{e} = {Mathf.Pow(2, entries.IndexOf(e))}")).Insert(0, $"\t{FlagsAttribute}{Environment.NewLine}");
                    }

                    default: return null;
                }
            }

            #endif
        }

        private string GetEnumString(string name, IEnumerable<string> entries)
        {
            const string NoneEntry = "None = 0";
            const string DeclarationSymbol = "public enum";
            string newLine = $"{Environment.NewLine}\t";
            string newEntryLine = $"{Environment.NewLine}\t";

            string declaration = $"\t{DeclarationSymbol} {name}";

            if (useNone) { entries = entries.Prepend(NoneEntry); }
            string body = string.Join($",{newEntryLine}", entries);

            return declaration
                + newLine
                + "{"
                + newLine
                + body
                + newLine
                + "};";
        }

        private void DeleteScriptEditor()
        {
            #if UNITY_EDITOR
            if (script == null) { return; }
            IO.Editor.Delete(script);
            #endif
        }
    }
}
