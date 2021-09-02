using System;
using System.Linq;
using System.Text;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
	public static partial class Serializer
    {
        public static partial class Bytes
        {
            public static class Size
            {
                public const int Byte = 1;
                public const int SByte = 1;
                public const int Bool = 1;
                public const int Short = 2;
                public const int UShort = 2;
                public const int Int = 4;
                public const int UInt = 4;
                public const int Long = 8;
                public const int ULong = 8;
                public const int Float = 4;
                public const int Double = 8;
                public const int Decimal = 16;
                public const int Char = 2;
            }
        }
    }
}