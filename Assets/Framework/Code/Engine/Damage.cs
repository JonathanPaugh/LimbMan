using System;
using System.Collections.Generic;

namespace Jape
{
    public abstract class Damage : Element.IReceivable
    {
        public static IEnumerable<Type> Classes() { return typeof(Damage).GetSubclass(); }
    }
}