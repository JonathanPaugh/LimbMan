using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
	public static partial class Serializer
    {
        public static partial class Dynamic
        {
            public class Writers : Allocator<Writer>
            {
                public Writers(int max) : base(null, Writer.OnRelease) { Max = max; }

                public byte[] ReleaseBytes(Writer writer)
                {
                    byte[] bytes = writer.ToArray();
                    Release(writer);
                    return bytes;
                }
            }
        }
    }
}