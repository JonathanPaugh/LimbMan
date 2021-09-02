using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jape;
using JetBrains.Annotations;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Jape
{
    public partial class Pack
    {
        public class Converter
        {
            public Type Type { get; }
            private Action<Pack, object> writer;
            private Func<Pack, object> reader;

            public Converter(Type type, Action<Pack, object> writer, Func<Pack, object> reader)
            {
                Type = type;
                this.writer = writer;
                this.reader = reader;
            }

            public void Write(Pack self, object value) { writer(self, value); }
            public object Read(Pack self) { return reader(self); }
        }
    }
}
