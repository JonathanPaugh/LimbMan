using System;

namespace Jape
{
	public partial class Entity
    {
        [Serializable]
        public new struct Send
        {
            public readonly Enum output;
            public readonly Type type;
            public readonly string name;

            public Send(Enum output, Type type, string name)
            {
                this.output = output;
                this.type = type;
                this.name = name;
            }
        }
    }
}