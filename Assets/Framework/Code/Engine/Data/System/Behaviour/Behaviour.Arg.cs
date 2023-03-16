using System;
using Sirenix.OdinInspector;

namespace Jape
{
    public partial class Behaviour
    {
        [Serializable]
        public class Arg : Jape.Arg
        {
            [PropertyOrder(-1)]
            public string member;
        }
    }
}