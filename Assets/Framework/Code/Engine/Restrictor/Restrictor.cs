using System.Collections.Generic;

namespace Jape
{
    public class Restrictor<T> : IRestrictor<T>
    {
        private List<T> restrictors = new();

        public bool IsRestricted()
        {
            return restrictors.Count != 0;
        }

        public void Restrict(T restrictor)
        {
            if (restrictors.Contains(restrictor)) { return; }
            restrictors.Add(restrictor);
        }

        public void Unrestrict(T restrictor)
        {
            if (!restrictors.Contains(restrictor)) { return; }
            restrictors.Remove(restrictor);
        }

        public void UnrestrictAll()
        {
            restrictors.Clear();
        }

        public override string ToString() { return $"Restrictor<{typeof(T)}>: {IsRestricted()}"; }
    }
}