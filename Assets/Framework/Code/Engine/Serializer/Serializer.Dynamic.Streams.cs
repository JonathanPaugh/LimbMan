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
            public class Streams : Allocator<SerialStream>
            {
                public Streams(int max) : base(null, SerialStream.OnRelease) { Max = max; }
            }
        }
    }
}