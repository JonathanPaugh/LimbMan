using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public class ScriptViewWindow : ViewWindow
    {
        private const float ButtonHeight = 32;

        protected override string Title => "Scripts";

        protected override float Width => 768;
        protected override float MinHeight => 512;

        protected override Action<object> AddAction => delegate(object parent)
        {
            ScriptCreateWindow window = Open<ScriptCreateWindow>();
            window.SetSelection(((Script)parent).name);
        };

        private static string ApplicationPath() { return Application.dataPath.Replace(@"/Assets", string.Empty); }

        protected override string GetParentLabel(object parent) { return ((Script)parent).name; }
        protected override string GetChildLabel(object child) { return Path.GetFileName((string)child); }

        protected override IEnumerable<object> ParentSelections() { return DataType.FindAll<Script>().Where(s => s.hidden == false); }
        protected override IEnumerable<object> ChildSelections(object parent) { return GetFiles((Script)parent); }

        protected IEnumerable<string> GetFiles(Script script)
        {
            List<string> files = new List<string>();

            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Engine));
            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Editor));
            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Net));

            if (Game.DeveloperMode)
            {
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Engine));
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Editor));
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Net));
            }

            return files;
        }

        protected IEnumerable<string> GetFile(Script script, Sector sector, CodeRegion region)
        {
            string path = $"{ApplicationPath()}/{script.GetFullPath(sector, region)}";
            return !System.IO.Directory.Exists(path) ? 
                   new string[0] : 
                   System.IO.Directory.GetFiles(path).Where(f => f.EndsWith(".cs")).Select(f => f.Replace(".cs", string.Empty));
        }

        protected TextAsset GetScript(string path)
        {
            return string.IsNullOrEmpty(path) ? 
                   null : 
                   AssetDatabase.LoadAssetAtPath<TextAsset>($"{path.Replace(@"\", "/").Replace(ApplicationPath(), string.Empty).Substring(1)}.cs");
        }

        protected override void DrawTree()
        {
            if (Child == null) { return; }

            GUILayout.BeginVertical();

            TextAsset selection = GetScript((string)Child);

            if (IsSet())
            {
                if (GUILayout.Button("Open", GUILayout.Height(ButtonHeight))) { AssetDatabase.OpenAsset(selection); }
                GUILayout.TextArea(selection.text);
            }

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
        }
    }
}