using System.Linq;

namespace Jape
{
    public partial class Modifier : Behaviour
    {
        protected new static string Path => IO.JoinPath(SystemPath, "Modifiers");

        protected override string NamePrefix() { return $"_{base.NamePrefix()}"; }

        public new static Modifier Find<T>() where T : ModifierInstance { return FindAll<Modifier>().FirstOrDefault(m => m.Type == typeof(T)); }
    }
}