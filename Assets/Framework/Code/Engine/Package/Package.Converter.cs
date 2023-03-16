using System;

namespace Jape
{
    public partial class Package
    {
        public class Converter
        {
            public Type Type { get; }
            private Action<Package, object> writer;
            private Func<Package, object> reader;

            public Converter(Type type, Action<Package, object> writer, Func<Package, object> reader)
            {
                Type = type;
                this.writer = writer;
                this.reader = reader;
            }

            public void Write(Package self, object value) { writer(self, value); }
            public object Read(Package self) { return reader(self); }
        }
    }
}
