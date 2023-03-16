using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jape
{
	public class FileWriter
    {
        private string content = string.Empty;
        private string @namespace = string.Empty;
        private string[] directives;
        private Func<int, string> inject;

        // ReSharper disable InconsistentNaming
        [Flags] public enum Term { SCRIPTNAME, NAMESPACE, DIRECTIVES, INJECT };
        // ReSharper restore InconsistentNaming

        public void WriteEditor(string assetPath)
        {
            #if UNITY_EDITOR

            string path = IO.JoinPath(Framework.ApplicationPath, assetPath);
            string directory = IO.GetDirectory(path);

            if (!IO.DirectoryExists(directory)) { this.Log().Response($"Cannot find directory: {directory}"); return; }

            IO.WriteText(path, ProcessEditor(assetPath));
            UnityEditor.AssetDatabase.Refresh();

            #endif
        }

        private string ProcessEditor(string assetPath)
        {
            #if UNITY_EDITOR

            if (string.IsNullOrEmpty(content)) { return content; }

            foreach (Term term in Enum.GetValues(typeof(Term)).Cast<Term>().Where(t => TermInstances(content, t).Count > 0))
            {
                switch (term)
                {
                    case Term.SCRIPTNAME: 
                        string name = IO.GetFileName(assetPath);
                        content = content.Replace($"#{term}#", name);
                        break;

                    case Term.NAMESPACE: 
                        content = content.Replace($"#{term}#", @namespace);
                        break;

                    case Term.DIRECTIVES:
                        if (directives == null || directives.Length == 0) { content = content.Replace($"{Environment.NewLine}#{term}#", string.Empty); break; }
                        content = content.Replace($"#{term}#", $"using {string.Join($";{Environment.NewLine}using ", directives)};");
                        break;

                    case Term.INJECT: 
                        MatchCollection injects = TermInstances(content, Term.INJECT);
                        for (int i = 0; i < injects.Count; i++) { content = content.Replace($"#{term}{i}#", inject.Invoke(i)); }
                        break;
                }
            }

            return content;

            #else

            return null;

            #endif
        }

        public FileWriter SetContent(string content) { this.content = content; return this; }
        public FileWriter SetNamespace(string @namespace) { this.@namespace = @namespace; return this; }
        public FileWriter SetDirectives(params string[] directives) { this.directives = directives; return this; }
        public FileWriter SetInject(Func<int, string> inject) { this.inject = inject; return this; }

        private static MatchCollection TermInstances(string content, Term term) { return Regex.Matches(content, $@"#{term}\d*#"); }
    }
}