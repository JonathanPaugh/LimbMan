using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;

namespace JapeEditor
{
    public class ScriptViewWindow : ViewWindow<Script, string>
    {
        private const float ButtonHeight = 32;

        protected override string Title => "Scripts";

        protected override float Width => 768;
        protected override float MinHeight => 512;

        private TextAsset targetScript;

        protected override Action<Script, string> AddAction => delegate(Script parent, string _)
        {
            ScriptCreateWindow window = Open<ScriptCreateWindow>();
            window.SetSelection(parent.name);
        };

        protected override string GetGroupLabel(Script group) { return group.name; }
        protected override string GetTargetLabel(string target) { return IO.GetFileName(target); }

        protected override IEnumerable<Script> GroupSelections() { return DataType.FindAll<Script>().Where(s => s.hidden == false); }
        protected override IEnumerable<string> TargetSelections(Script parent) { return GetFiles(parent); }

        protected IEnumerable<string> GetFiles(Script script)
        {
            List<string> files = new();

            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Engine));
            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Editor));
            files.AddRange(GetFile(script, Sector.Game, CodeRegion.Net));

            if (Framework.DeveloperMode)
            {
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Engine));
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Editor));
                files.AddRange(GetFile(script, Sector.Framework, CodeRegion.Net));
            }

            return files;
        }

        protected IEnumerable<string> GetFile(Script script, Sector sector, CodeRegion region)
        {
            string path = IO.JoinPath(Framework.ApplicationPath, script.GetFullPath(sector, region));
            return !IO.DirectoryExists(path) ? 
                   Array.Empty<string>() : 
                   IO.GetDirectoryFiles(path).Where(f => f.EndsWith(".cs"));
        }

        protected TextAsset GetScript(string path)
        {
            return string.IsNullOrEmpty(path) ? 
                   null : 
                   AssetDatabase.LoadAssetAtPath<TextAsset>($"{path.Replace($"{Framework.ApplicationPath}{IO.DirectorySperator}", string.Empty)}");
        }

        protected override void DrawTarget()
        {
            GUILayout.BeginHorizontal();

            DrawInspectButton(targetScript);
            if (GUILayout.Button("Open", GUILayout.Height(ButtonHeight))) { AssetDatabase.OpenAsset(targetScript); }

            GUILayout.EndHorizontal();

            GUILayout.TextArea(targetScript.text);
        }

        protected override void OnSelectTarget(string target)
        {
            targetScript = GetScript(target);
        }
    }
}