using System;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public readonly struct Encodable
    {
        private const char Seperator = '_';

        private readonly Segment[] segments;

        public Encodable(params Segment[] segments)
        {
            this.segments = segments;
        }

        public byte[] Encode()
        {
            List<byte> bytes = new List<byte>();
            foreach (Segment segment in segments)
            {
                bytes.AddRange(segment.Encode());
            }
            return bytes.ToArray();
        }

        public override string ToString() => string.Join(Seperator.ToString(), segments.Select(segment => segment.ToString()));

        public readonly struct Segment
        {
            private readonly string key;
            private readonly Func<string, byte[]> toBytes;

            public Segment(string key, Func<string, byte[]> toBytes)
            {
                this.key = key;
                this.toBytes = toBytes;
            }

            public byte[] Encode() => toBytes.Invoke(key);

            public override string ToString() => key;
        }
    }
}