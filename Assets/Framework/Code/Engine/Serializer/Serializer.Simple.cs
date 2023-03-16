using System;
using System.Collections.Generic;

namespace Jape
{
	public static partial class Serializer
    {
        public static partial class Simple
        {
            private const byte IndexNull = 0;
            private const byte IndexByte = 1;
            private const byte IndexSByte = 2;
            private const byte IndexBool = 3;
            private const byte IndexShort = 4;
            private const byte IndexUShort = 5;
            private const byte IndexInt = 6;
            private const byte IndexUInt = 7;
            private const byte IndexLong = 8;
            private const byte IndexULong = 9;
            private const byte IndexFloat = 10;
            private const byte IndexDouble = 11;
            private const byte IndexDecimal = 12;
            private const byte IndexChar = 13;
            private const byte IndexString = 14;

            private static List<Converter> converters = new()
            {
                new Converter(null, null, null),
                new Converter(typeof(byte), o => ToBytes((byte) o), ToByte),
                new Converter(typeof(sbyte), o => ToBytes((sbyte) o), ToSByte),
                new Converter(typeof(bool), o => ToBytes((bool) o), ToBool),
                new Converter(typeof(short), o => ToBytes((short) o), ToShort),
                new Converter(typeof(ushort), o => ToBytes((ushort) o), ToUShort),
                new Converter(typeof(int), o => ToBytes((int) o), ToInt),
                new Converter(typeof(uint), o => ToBytes((uint) o), ToUInt),
                new Converter(typeof(long), o => ToBytes((long) o), ToLong),
                new Converter(typeof(ulong), o => ToBytes((ulong) o), ToULong),
                new Converter(typeof(float), o => ToBytes((float) o), ToFloat),
                new Converter(typeof(double), o => ToBytes((double) o), ToDouble),
                new Converter(typeof(decimal), o => ToBytes((decimal) o), ToDecimal),
                new Converter(typeof(char), o => ToBytes((char) o), ToChar),
                new Converter(typeof(string), o => ToBytes((string) o), ToString),
            };

            private static byte[] temp;

            public static byte[] Serialize<T>(T value)
            {
                return SerializeWeak(value);
            }

            public static byte[] SerializeWeak(object value)
            {
                if (value == null)
                {
                    return new [] { IndexNull };
                }

                Type dataType = value.GetType();

                foreach (Converter converter in converters)
                {
                    if (converter.Type != dataType) { continue; }
                    return converter.Serialize(value);
                }

                Log.Warning($"Cannot serialize type: {dataType}");

                return default;
            }

            public static T Deserialize<T>(byte[] data, int position = 0)
            {
                object value = DeserializeWeak(data, position);

                try
                {
                    return (T)value;
                }
                catch (Exception e)
                {
                    Log.Warning($"Cannot convert deserialized {value.GetType()} to {typeof(T)}");
                    Log.Error(e);
                    return default;
                }
            }

            public static object DeserializeWeak(byte[] data, int position = 0)
            {
                DeserializeInternal(data, out object value, position);
                return value;
            }

            internal static int DeserializeInternal(byte[] data, out object value, int position = 0)
            {
                byte index = Bytes.ToByte(data, position);
                position += Bytes.Size.Byte;

                if (index == IndexNull)
                {
                    value = null;
                    return position;
                }

                try
                {
                    position = converters[index].Deserialize(data, out value, position);
                    return position;
                }
                catch (Exception e)
                {
                    Log.Warning("Cannot deserialize data");
                    Log.Error(e);
                    value = default;
                    return position;
                }
            }

            private static byte[] ToBytes(byte value)
            {
                byte[] bytes =
                {
                    IndexByte,
                    value
                };
                return bytes;
            }

            private static byte[] ToBytes(sbyte value)
            {
                byte[] bytes =
                {
                    IndexSByte,
                    (byte)value
                };
                return bytes;
            }

            private static byte[] ToBytes(bool value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexBool,
                    temp[0]
                };
                return bytes;
            }

            private static byte[] ToBytes(short value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexShort,
                    temp[0],
                    temp[1]
                };
                return bytes;
            }

            private static byte[] ToBytes(ushort value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexUShort,
                    temp[0],
                    temp[1]
                };
                return bytes;
            }

            private static byte[] ToBytes(int value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexInt,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                };
                return bytes;
            }

            private static byte[] ToBytes(uint value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexUInt,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                };
                return bytes;
            }

            private static byte[] ToBytes(long value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexLong,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                    temp[4],
                    temp[5],
                    temp[6],
                    temp[7],
                };
                return bytes;
            }

            private static byte[] ToBytes(ulong value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexULong,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                    temp[4],
                    temp[5],
                    temp[6],
                    temp[7],
                };
                return bytes;
            }

            private static byte[] ToBytes(float value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexFloat,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                };
                return bytes;
            }

            private static byte[] ToBytes(double value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexDouble,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                    temp[4],
                    temp[5],
                    temp[6],
                    temp[7],
                };
                return bytes;
            }

            private static byte[] ToBytes(decimal value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexDecimal,
                    temp[0],
                    temp[1],
                    temp[2],
                    temp[3],
                    temp[4],
                    temp[5],
                    temp[6],
                    temp[7],
                    temp[8],
                    temp[9],
                    temp[10],
                    temp[11],
                    temp[12],
                    temp[13],
                    temp[14],
                    temp[15],
                };
                return bytes;
            }

            private static byte[] ToBytes(char value)
            {
                temp = Bytes.Get(value);
                byte[] bytes =
                {
                    IndexChar,
                    temp[0],
                    temp[1],
                };
                return bytes;
            }

            private static byte[] ToBytes(string value)
            {
                byte[] data = Bytes.Get(value);
                temp = Bytes.Get(data.Length);
                byte[] bytes = new byte[data.Length + Bytes.Size.Byte + Bytes.Size.Int];

                bytes[0] = IndexString;

                int offset = Bytes.Size.Byte;
                for (int i = 0; i < Bytes.Size.Int; i++)
                {
                    bytes[i + offset] = temp[i];
                }

                offset += Bytes.Size.Int;
                for (int i = 0; i < data.Length; i++)
                {
                    bytes[i + offset] = data[i];
                }

                return bytes;
            }

            private static int ToByte(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToByte(bytes, position);
                return position + Bytes.Size.Byte;
            }

            private static int ToSByte(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToSByte(bytes, position);
                return position + Bytes.Size.SByte;
            }

            private static int ToBool(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToBool(bytes, position);
                return position + Bytes.Size.Bool;
            }

            private static int ToShort(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToShort(bytes, position);
                return position + Bytes.Size.Short;
            }

            private static int ToUShort(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToUShort(bytes, position);
                return position + Bytes.Size.UShort;
            }

            private static int ToInt(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToInt(bytes, position);
                return position + Bytes.Size.Int;
            }

            private static int ToUInt(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToUInt(bytes, position);
                return position + Bytes.Size.UInt;
            }

            private static int ToLong(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToLong(bytes, position);
                return position + Bytes.Size.Long;
            }

            private static int ToULong(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToULong(bytes, position);
                return position + Bytes.Size.ULong;
            }

            private static int ToFloat(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToFloat(bytes, position);
                return position + Bytes.Size.Float;
            }

            private static int ToDouble(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToDouble(bytes, position);
                return position + Bytes.Size.Double;
            }

            private static int ToDecimal(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToDecimal(bytes, position);
                return position + Bytes.Size.Decimal;
            }

            private static int ToChar(byte[] bytes, out object value, int position)
            {
                value = Bytes.ToChar(bytes, position);
                return position + Bytes.Size.Char;
            }

            private static int ToString(byte[] bytes, out object value, int position)
            {
                int length = Bytes.ToInt(bytes, position);
                position += Bytes.Size.Int;
                value = Bytes.ToString(bytes, length, position);
                return position + length;
            }
        }
    }
}