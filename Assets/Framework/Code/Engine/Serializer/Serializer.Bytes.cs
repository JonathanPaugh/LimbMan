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
            public static byte[] Offset(byte[] data, int offset)
            {
                byte[] bytes = new byte[data.Length - offset];
                Buffer.BlockCopy(data, offset, bytes, 0, bytes.Length);
                return bytes;
            }

            public static byte[] Combine(params byte[][] arrays)
            {
                byte[] bytes = new byte[arrays.Sum(x => x.Length)];
                int offset = 0;
                foreach (byte[] data in arrays)
                {
                    Buffer.BlockCopy(data, 0, bytes, offset, data.Length);
                    offset += data.Length;
                }
                return bytes;
            }

            public static byte[] Get(byte value)
            {
                return new [] { value };
            }

            public static byte[] Get(sbyte value)
            {
                return new [] { (byte)value };
            }

            public static byte[] Get(bool value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(short value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(ushort value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(int value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(uint value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(long value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(ulong value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(float value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(double value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(decimal value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default:
                    {
                        int[] bits = decimal.GetBits(value);
                        return Combine(Get(bits[0]), Get(bits[1]), Get(bits[2]), Get(bits[3]));
                    }
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(char value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte[] Get(string value)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return Encoding.UTF8.GetBytes(value);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static byte ToByte(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return bytes[position];
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static sbyte ToSByte(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return (sbyte)bytes[position];
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static bool ToBool(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToBoolean(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static short ToShort(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToInt16(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static ushort ToUShort(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToUInt16(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static int ToInt(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToInt32(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static uint ToUInt(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToUInt32(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static long ToLong(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToInt64(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static ulong ToULong(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToUInt64(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static float ToFloat(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToSingle(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static double ToDouble(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToDouble(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static decimal ToDecimal(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default:
                    {
                        int[] bits =
                        {
                            ToInt(bytes, position + (Size.Int * 0)), 
                            ToInt(bytes, position + (Size.Int * 1)), 
                            ToInt(bytes, position + (Size.Int * 2)), 
                            ToInt(bytes, position + (Size.Int * 3))
                        };
                        return new decimal(bits);
                    }
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static char ToChar(byte[] bytes, int position = 0)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return System.BitConverter.ToChar(bytes, position);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }

            public static string ToString(byte[] bytes, int length, int position = 4)
            {
                switch (Settings.bitConverter)
                {
                    case BitConverter.Default: return Encoding.UTF8.GetString(bytes, position, length);
                    default: throw new ArgumentOutOfRangeException();   
                }
            }
        }
    }
}