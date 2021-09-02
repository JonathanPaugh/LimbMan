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

        #if UNITY_EDITOR

        public void Write(string pathName)
        {
            string directory = $"{Directory.SystemPath()}{pathName.Remove(pathName.LastIndexOf('/') + 1)}";
            if (!System.IO.Directory.Exists(directory)) { this.Log().Response($"Cannot find directory: {directory}"); return; }
            System.IO.File.WriteAllText($"{Directory.SystemPath()}{pathName}", Process(pathName));
            UnityEditor.AssetDatabase.Refresh();
        }

        private string Process(string pathName)
        {
            if (string.IsNullOrEmpty(content)) { return content; }
            foreach (Term term in Enum.GetValues(typeof(Term)).Cast<Term>().Where(t => TermInstances(content, t).Count > 0))
            {
                switch (term)
                {
                    case Term.SCRIPTNAME: 
                        string name = pathName.Substring(pathName.LastIndexOf('/') + 1);
                        name = name.Remove(name.IndexOf('.'));
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
        }

        public FileWriter SetContent(string content) { this.content = content; return this; }
        public FileWriter SetNamespace(string @namespace) { this.@namespace = @namespace; return this; }
        public FileWriter SetDirectives(params string[] directives) { this.directives = directives; return this; }
        public FileWriter SetInject(Func<int, string> inject) { this.inject = inject; return this; }

        private static MatchCollection TermInstances(string content, Term term) { return Regex.Matches(content, $@"#{term}\d*#"); }

        #endif
    }
}