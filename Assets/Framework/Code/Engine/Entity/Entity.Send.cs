using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Jape
{
	public partial class Entity
    {
        [Serializable]
        public new struct Send
        {
            public readonly Enum output;
            public readonly System.Type type;
            public readonly string name;

            public Send(Enum output, System.Type type, string name)
            {
                this.output = output;
                this.type = type;
                this.name = name;
            }
        }
    }
}