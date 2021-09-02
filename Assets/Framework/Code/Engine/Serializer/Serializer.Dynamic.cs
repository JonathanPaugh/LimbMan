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
            private static Writers writers = new Writers(Settings.writerLimit);
            private static Streams streams = new Streams(Settings.streamLimit);

            public static void ShrinkAllocation()
            {
                writers.Shrink();
                streams.Shrink();
            }

            public static void ExpandAllocation()
            {
                writers.Expand();
                streams.Expand();
            }

            public static byte[] Serialize<T>(T value)
            {
                return SerializeWeak(value);
            }

            public static byte[] SerializeWeak(object value)
            {
                Writer data = writers.Catch();

                if (value == null)
                {
                    data.Write(0);
                }
                else
                {
                    switch (value)
                    {
                        case ISerialStreamable streamable:
                        {
                            data.Write(5);
                            WriteType();
                            data.Write(WriteStreamable(streamable));
                            break;
                        }

                        case ISerializable serializable:
                        {
                            data.Write(4);
                            WriteType();
                            data.Write(WriteSerializable(serializable));
                            break;
                        }

                        case IDictionary dictionary:
                        {
                            data.Write(3);
                            WriteType();
                            data.Write(WriteDictionary(dictionary));
                            break;
                        }

                        case IList list:
                        {
                            data.Write(2);
                            WriteType();
                            data.Write(WriteList(list));
                            break;
                        }

                        default:
                        {
                            data.Write(1);
                            WriteType();
                            data.Write(WriteValue(value));
                            break;
                        }
                    }
                }

                return writers.ReleaseBytes(data);

                void WriteType()
                {
                    byte[] typeBytes = Bytes.Get(value.GetType().AssemblyQualifiedName);
                    data.Write(Bytes.Get(typeBytes.Length));
                    data.Write(typeBytes);
                }
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

                if (index == 0)
                {
                    value = null;
                }
                else
                {
                    try
                    {
                        Type type = ReadType();
                        switch (index)
                        {
                            case 5:
                                position = ReadStreamable(data, type, out ISerialStreamable streamable, position);
                                value = streamable;
                                break;

                            case 4:
                                position = ReadSerializable(data, type, out ISerializable serializable, position);
                                value = serializable;
                                break;

                            case 3:
                                position = ReadDictionary(data, type, out IDictionary dictionary, position);
                                value = dictionary;
                                break;

                            case 2:
                                position = ReadList(data, type, out IList list, position);
                                value = list;
                                break;

                            default:
                                position = ReadValue(data, out object temp, position);
                                value = temp;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Cannot deserialize data");
                        Log.Error(e);
                        value = null;
                        return position;
                    }
                }
                
                return position;

                Type ReadType()
                {
                    int typeLength = Bytes.ToInt(data, position);
                    position += Bytes.Size.Int;

                    string typeName = Bytes.ToString(data, typeLength, position);
                    position += typeLength;

                    return Type.GetType(typeName, true);
                }
            }

            public static byte[] WriteValue(object value)
            {
                return Simple.SerializeWeak(value);
            }

            public static int ReadValue<T>(byte[] data, out T value, int position = 0)
            {
                int length = ReadValue(data, out object temp, position);
                value = (T)temp;
                return length;
            }

            public static int ReadValue(byte[] data, out object value, int position = 0)
            {
                return Simple.DeserializeInternal(data, out value, position);
            }

            public static byte[] WriteList(IList list)
            {
                Writer data = writers.Catch();

                data.Write(Bytes.Get(list.Count));
                foreach (object entry in list)
                {
                    data.Write(Serialize(entry));
                }

                return writers.ReleaseBytes(data);
            }

            public static int ReadList<T>(byte[] data, out T list, int position = 0) where T : IList
            {
                int length = ReadList(data, typeof(T), out IList value, position);
                list = (T)value;
                return length;
            }

            public static int ReadList(byte[] data, Type type, out IList list, int position = 0)
            {
                int count = Bytes.ToInt(data, position);
                position += Bytes.Size.Int;

                list = Activator.CreateInstance(type, count) as IList;
                
                for (int i = 0; i < count; i++)
                {
                    position = Serializer.DeserializeInternal(data, out object temp, position);
                    object value = temp;

                    if (list.IsFixedSize) { list[i] = value; } 
                    else { list.Add(value); }
                }
                        
                return position;
            }

            public static byte[] WriteDictionary(IDictionary dictionary)
            {
                Writer data = writers.Catch();

                data.Write(Bytes.Get(dictionary.Count));
                foreach (DictionaryEntry entry in dictionary)
                {
                    data.Write(Serialize(entry.Key));
                    data.Write(Serialize(entry.Value));
                }

                return writers.ReleaseBytes(data);
            }

            public static int ReadDictionary<T>(byte[] data, out T dictionary, int position = 0) where T : IDictionary
            {
                int length = ReadDictionary(data, typeof(T), out IDictionary value, position);
                dictionary = (T)value;
                return length;
            }

            public static int ReadDictionary(byte[] data, Type type, out IDictionary dictionary, int position = 0)
            {
                int count = Bytes.ToInt(data, position);
                position += Bytes.Size.Int;

                dictionary = Activator.CreateInstance(type, count) as IDictionary;
                
                for (int i = 0; i < count; i++)
                {
                    position = Serializer.DeserializeInternal(data, out object temp1, position);
                    object key = temp1;

                    position = Serializer.DeserializeInternal(data, out object temp2, position);
                    object value = temp2;

                    if (dictionary.IsFixedSize) { dictionary[i] = value; } 
                    else { dictionary.Add(key, value); }
                }

                return position;
            }

            public static byte[] WriteSerializable(ISerializable serializable)
            {
                return serializable.Serialize();
            }

            public static int ReadSerializable(byte[] data, Type type, out ISerializable value, int position = 0)
            {
                value = Activator.CreateInstance(type) as ISerializable;
                position = value.Deserialize(data, position);
                return position;
            }

            public static byte[] WriteStreamable(ISerialStreamable streamable)
            {
                SerialStream stream = streams.Catch();
                stream.StartWriting();
                streamable.SerialStream(stream);
                stream.Stop();

                return WriteList(stream.ToArray());
            }

            public static int ReadStreamable(byte[] data, Type type, out ISerialStreamable value, int position = 0)
            {
                position = ReadList(data, out object[] values, position);

                value = Activator.CreateInstance(type) as ISerialStreamable;

                SerialStream stream = streams.Catch();
                stream.SetData(values);
                stream.StartReading();
                value.SerialStream(stream);
                stream.Stop();

                return position;
            }
        }
    }
}