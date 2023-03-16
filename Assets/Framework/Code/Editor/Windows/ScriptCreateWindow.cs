using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector;

namespace JapeEditor
{
    public class ScriptCreateWindow : CreateWindow
    {
        protected override string Title => "Create Script";

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;

        protected override float Width => 384;

        [PropertyOrder(-1)]
        [EnableIf(Framework.FrameworkDeveloperMode)]
        [HideLabel]
        [EnumToggleButtons]
        public Sector sector = Sector.Game;

        protected override Action<object> CreateAction => delegate(object selection)
        {
            if (!GetScriptReferences().TryGetValue((string)selection, out Reference reference)) { return; }
            reference.Script.CreateEditor(sector, reference.CodeRegion);
        };

        protected override IList<object> Selections() { return GetScriptNames().Cast<object>().ToList(); }

        protected IEnumerable<string> GetScriptNames()
        {
            Dictionary<string, Reference> references = GetScriptReferences();
            List<string> names = new();
            names.AddRange(references.Where(r => r.Value.CodeRegion == CodeRegion.Engine).Select(r => r.Key));
            names.AddRange(references.Where(r => r.Value.CodeRegion == CodeRegion.Editor).Select(r => r.Key));
            names.AddRange(references.Where(r => r.Value.CodeRegion == CodeRegion.Net).Select(r => r.Key));
            return names;
        }

        protected Dictionary<string, Reference> GetScriptReferences()
        {
            Dictionary<string, Reference> references = new();
            foreach (Script script in Script.GetScripts(Script.Mode.Script, sector))
            {
                if (script.GetRegions().HasFlag(CodeRegionFlags.Engine)) { references.Add($"Engine: {script.name}", new Reference(script, CodeRegion.Engine)); }
                if (script.GetRegions().HasFlag(CodeRegionFlags.Editor)) { references.Add($"Editor: {script.name}", new Reference(script, CodeRegion.Editor)); }
                if (script.GetRegions().HasFlag(CodeRegionFlags.Net)) { references.Add($"Net: {script.name}", new Reference(script, CodeRegion.Net)); }
            }
            return references;
        }

        [PropertyOrder(1)]
        [ShowInInspector]
        [HideLabel, ReadOnly]
        [ShowIf(nameof(Path))]
        public string Path
        {
            get
            {
                if (!IsSet()) { return null; }
                if (!GetScriptReferences().TryGetValue((string)Selection, out Reference reference)) { return null; }
                string path =  reference.Script.GetFullPath(sector, reference.CodeRegion);
                return path ?? $"Selection: {IO.Editor.SelectionFolder}";
            }
        }

        [MenuItem("Assets/Create/Script", false, -9)]
        private static void Menu() { Open<ScriptCreateWindow>(); }

        public readonly struct Reference
        {
            public Reference(Script script, CodeRegion codeRegion)
            {
                Script = script;
                CodeRegion = codeRegion;
            }

            public Script Script { get; }
            public CodeRegion CodeRegion { get; }
        }
    }
}