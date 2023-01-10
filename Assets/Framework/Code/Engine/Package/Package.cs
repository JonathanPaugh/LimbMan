using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
    public partial class Package : IDisposable
    {
        private List<byte> bytes;
        private byte[] readableBytes;
        private int position;

        private static List<Converter> converters = new List<Converter>
        {
            new Converter(typeof(byte[]), (b, o) =>
            {
                byte[] bytes = (byte[])o;
                b.Write(bytes.Length);
                b.Write(bytes);
            }, b => b.ReadBytes(b.ReadInt())),
            new Converter(typeof(byte), (b, o) => b.Write((byte)o), b => b.ReadByte()),
            new Converter(typeof(sbyte), (b, o) => b.Write((sbyte)o), b => b.ReadSByte()),
            new Converter(typeof(bool), (b, o) => b.Write((bool)o), b => b.ReadBool()),
            new Converter(typeof(short), (b, o) => b.Write((short)o), b => b.ReadShort()),
            new Converter(typeof(ushort), (b, o) => b.Write((ushort)o), b => b.ReadUShort()),
            new Converter(typeof(int), (b, o) => b.Write((int)o), b => b.ReadInt()),
            new Converter(typeof(uint), (b, o) => b.Write((uint)o), b => b.ReadUInt()),
            new Converter(typeof(long), (b, o) => b.Write((long)o), b => b.ReadLong()),
            new Converter(typeof(ulong), (b, o) => b.Write((ulong)o), b => b.ReadULong()),
            new Converter(typeof(float), (b, o) => b.Write((float)o), b => b.ReadFloat()),
            new Converter(typeof(double), (b, o) => b.Write((double)o), b => b.ReadDouble()),
            new Converter(typeof(decimal), (b, o) => b.Write((decimal)o), b => b.ReadDecimal()),
            new Converter(typeof(char), (b, o) => b.Write((char)o), b => b.ReadChar()),
            new Converter(typeof(string), (b, o) => b.Write((string)o), b => b.ReadString()),
            new Converter(typeof(Vector2), (b, o) => b.Write((Vector2)o), b => b.ReadVector2()),
            new Converter(typeof(Vector3), (b, o) => b.Write((Vector3)o), b => b.ReadVector3()),
            new Converter(typeof(Quaternion), (b, o) => b.Write((Quaternion)o), b => b.ReadQuaternion()),
            new Converter(typeof(Color), (b, o) => b.Write((Color)o), b => b.ReadColor()),
            new Converter(typeof(Mono.Key), (b, o) => b.Write((Mono.Key)o), b => b.ReadColor()),
        };

        public Package()
        {
            bytes = new List<byte>();
            position = 0;
        }

        public Package(byte[] data)
        {
            bytes = new List<byte>();
            position = 0;

            SetBytes(data);
        }

        private static bool CanConvert(Type type)
        {
            return converters.Any(c => c.Type == type);
        }

        public void SetBytes(byte[] data)
        {
            Write(data);
            readableBytes = bytes.ToArray();
        }

        public void InsertLength()
        {
            bytes.InsertRange(0, Serializer.Bytes.Get(bytes.Count));
        }

        public void InsertInt(int value)
        {
            bytes.InsertRange(0, Serializer.Bytes.Get(value));
        }

        public byte[] ToArray()
        {
            readableBytes = bytes.ToArray();
            return readableBytes;
        }

        public int Length()
        {
            return bytes.Count;
        }

        public int LengthRemaining()
        {
            return Length() - position;
        }

        public void Reset(bool resetBuffer = true)
        {
            if (resetBuffer)
            {
                bytes.Clear();
                readableBytes = null;
                position = 0;
            }
            else
            {
                position -= 4;
            }
        }

        public void Write(object value)
        {
            Type type = value?.GetType();
            if (type != null && CanConvert(type))
            {
                Write(true);
                for (int i = 0; i < converters.Count; i++)
                {
                    if (converters[i].Type != type) { continue; }
                    Write(i);
                    converters[i].Write(this, value);
                    break;
                }
            }
            else
            {
                Write(false);
                byte[] bytes = Serializer.Serialize(value);
                Write(bytes.Length);
                Write(bytes);
            }
        }

        public void Write(object[] value)
        {
            Write(value.Length);
            foreach (object item in value) { Write(item); }
        }

        public void Write(byte[] value) { bytes.AddRange(value); }
        public void Write(byte value) { bytes.Add(value); }
        public void Write(sbyte value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(bool value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(short value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(ushort value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(int value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(uint value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(long value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(ulong value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(float value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(double value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(decimal value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(char value) { bytes.AddRange(Serializer.Bytes.Get(value)); }
        public void Write(string value)
        {
            byte[] data = Serializer.Bytes.Get(value);
            bytes.AddRange(Serializer.Bytes.Get(data.Length));
            bytes.AddRange(data);
        }

        public void Write(Vector2 value)
        {
            Write(value.x);
            Write(value.y);
        }

        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        public void Write(Color value)
        {
            Write(value.r);
            Write(value.g);
            Write(value.b);
            Write(value.a);
        }

        public void Write(Mono.Key value)
        {
            byte[] bytes = value.Encode();
            Write(bytes.Length);
            Write(bytes);
        }

        public object ReadObject()
        {
            if (IsConverted())
            {
                int index = ReadInt();
                return converters[index].Read(this);
            } 
            else
            {
                int length = ReadInt();
                return Serializer.Deserialize<object>(ReadBytes(length));
            }

            bool IsConverted() => ReadBool();
        }

        public object[] ReadObjects()
        {
            int count = ReadInt();
            object[] value = new object[count];
            for (int i = 0; i < count; i++) { value[i] = ReadObject(); }
            return value;
        }

        public byte[] ReadBytes(int length, bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                byte[] value = bytes.GetRange(position, length).ToArray();
                if (moveNext)
                {
                    position += length;
                }
                return value;
            }

            throw new Exception("Unable to read byte[]");
        }

        public byte ReadByte(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                byte value = readableBytes[position];
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Byte;
                }
                return value;
            }

            throw new Exception("Unable to read byte");
        }

        public sbyte ReadSByte(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                sbyte value = (sbyte)readableBytes[position];
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.SByte;
                }
                return value;
            }

            throw new Exception("Unable to read sbyte");
        }

        public bool ReadBool(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                bool value = Serializer.Bytes.ToBool(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Bool;
                }
                return value;
            }

            throw new Exception("Unable to read bool");
        }

        public short ReadShort(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                short value = Serializer.Bytes.ToShort(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Short;
                }
                return value;
            }

            throw new Exception("Unable to read short");
        }

        public ushort ReadUShort(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                ushort value = Serializer.Bytes.ToUShort(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.UShort;
                }
                return value;
            }

            throw new Exception("Unable to read ushort");
        }

        public int ReadInt(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                int value = Serializer.Bytes.ToInt(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Int;
                }
                return value;
            }

            throw new Exception("Unable to read int");
        }

        public uint ReadUInt(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                uint value = Serializer.Bytes.ToUInt(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.UInt;
                }
                return value;
            }

            throw new Exception("Unable to read uint");
        }

        public long ReadLong(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                long value = Serializer.Bytes.ToLong(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Long;
                }
                return value;
            }

            throw new Exception("Unable to read long");
        }

        public ulong ReadULong(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                ulong value = Serializer.Bytes.ToULong(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.ULong;
                }
                return value;
            }

            throw new Exception("Unable to read ulong");
        }

        public float ReadFloat(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                float value = Serializer.Bytes.ToFloat(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Float;
                }
                return value;
            }

            throw new Exception("Unable to read float");
        }

        public double ReadDouble(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                double value = Serializer.Bytes.ToDouble(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Double;
                }
                return value;
            }

            throw new Exception("Unable to read double");
        }

        public decimal ReadDecimal(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                decimal value = Serializer.Bytes.ToDecimal(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Decimal;
                }
                return value;
            }

            throw new Exception("Unable to read decimal");
        }

        public double ReadChar(bool moveNext = true)
        {
            if (bytes.Count > position)
            {
                double value = Serializer.Bytes.ToChar(readableBytes, position);
                if (moveNext)
                {
                    position += Serializer.Bytes.Size.Char;
                }
                return value;
            }

            throw new Exception("Unable to read double");
        }

        public string ReadString(bool moveNext = true)
        {
            try
            {
                int length = ReadInt();
                string value = Serializer.Bytes.ToString(readableBytes, length, position);
                if (moveNext && value.Length > 0)
                {
                    position += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Unable to read string");
            }
        }

        public Vector2 ReadVector2(bool moveNext = true)
        {
            return new Vector2(ReadFloat(moveNext), ReadFloat(moveNext));
        }

        public Vector3 ReadVector3(bool moveNext = true)
        {
            return new Vector3(ReadFloat(moveNext), ReadFloat(moveNext), ReadFloat(moveNext));
        }

        public Quaternion ReadQuaternion(bool moveNext = true)
        {
            return new Quaternion(ReadFloat(moveNext), ReadFloat(moveNext), ReadFloat(moveNext), ReadFloat(moveNext));
        }

        public Color ReadColor(bool moveNext = true)
        {
            return new Color(ReadFloat(moveNext), ReadFloat(moveNext), ReadFloat(moveNext), ReadFloat(moveNext));
        }

        public byte[] ReadMonoKey(bool moveNext = true)
        {
            return ReadBytes(ReadInt(), moveNext);
        }

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    bytes = null;
                    readableBytes = null;
                    position = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
