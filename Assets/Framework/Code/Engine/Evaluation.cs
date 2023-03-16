using System;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public class Evaluation
    {
        public List<Func<bool>> predicates = new();

        public Evaluation(params Func<bool>[] requirements) { predicates.AddRange(requirements); }

        public bool Evaluate() { return predicates.All(predicate => predicate()); }

        public static implicit operator bool(Evaluation evaluation) { return evaluation != null && evaluation.Evaluate(); }
    }
}