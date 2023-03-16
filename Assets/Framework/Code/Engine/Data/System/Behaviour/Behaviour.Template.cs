using System;

namespace Jape
{
    public partial class Behaviour
    {
        public class Template : Jape.Template
        {
            protected override string Extension => "cs";
            protected override bool ShowTerms => false;

            public string Prefix { get; set; } = string.Empty;
            public string Suffix { get; set; } = string.Empty;

            public Func<CodeRegion> Region { get; set; }

            public Script Script { get; set; }
            public Func<int, string> Inject { get; set; }

            protected override string Name
            {
                get
                {
                    if (IsSet())
                    {
                        name = template.name; 
                        return name;
                    }
                    return $"{Prefix}{name}{Suffix}";
                }
                set
                {
                    name = value ?? string.Empty;
                    if (!string.IsNullOrEmpty(Prefix)) { name = name.Replace(Prefix, string.Empty); }
                    if (!string.IsNullOrEmpty(Suffix)) { name = name.Replace(Suffix, string.Empty); }
                    name = name.Replace(" ", string.Empty);
                }
            }

            public override void CreateEditor()
            {
                Script.Inject = Inject;
                template = Script.CreateAssetEditor(GetSector(), GetRegion(), Name);
            }

            protected CodeRegion GetRegion() { return Region?.Invoke() ?? CodeRegion.Engine; }

            public void SetName(string name) { Name = name; }

            public void SetInject(Func<int, string> inject) { Inject = inject; }
        }
    }
}