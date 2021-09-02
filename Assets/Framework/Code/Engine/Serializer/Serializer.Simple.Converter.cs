using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Jape
{
	public static partial class Serializer
    {
        public static partial class Simple
        {
            public class Converter
            {
                public Type Type { get; }
                private Serializer serializer;
                private Deserializer deserializer;

                public delegate byte[] Serializer(object value);
                public delegate int Deserializer(byte[] data, out object value, int position = 0);

                public Converter(Type type, Serializer serializer, Deserializer deserializer)
                {
                    Type = type;
                    this.serializer = serializer;
                    this.deserializer = deserializer;
                }

                public byte[] Serialize(object value) { return serializer(value); }

                public int Deserialize(byte[] data, out object value, int position)
                {
                    position = deserializer(data, out object temp, position);
                    value = temp;
                    return position;
                }
            }
        }
    }
}