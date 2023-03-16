using Sirenix.Serialization;

namespace Jape
{
	public static partial class Serializer
    {
        public static class Odin
        {
            public static byte[] Serialize<T>(T value)
            {
                byte[] bytes = SerializationUtility.SerializeValue(value, DataFormat.Binary);
                return Bytes.Combine(Bytes.Get(bytes.Length), bytes);
            }

            public static byte[] SerializeWeak(object value)
            {
                byte[] bytes =  SerializationUtility.SerializeValueWeak(value, DataFormat.Binary);
                return Bytes.Combine(Bytes.Get(bytes.Length), bytes);
            }

            public static T Deserialize<T>(byte[] data)
            {
                return SerializationUtility.DeserializeValue<T>(Bytes.Offset(data, Bytes.Size.Int), DataFormat.Binary);
            }

            public static object DeserializeWeak(byte[] data)
            {
                return SerializationUtility.DeserializeValueWeak(Bytes.Offset(data, Bytes.Size.Int), DataFormat.Binary);
            }

            public static int DeserializeInternal(byte[] data, out object value, int position = 0)
            {
                position += Bytes.ToInt(data, position);
                position += Bytes.Size.Int;

                value = SerializationUtility.DeserializeValueWeak(Bytes.Offset(data, Bytes.Size.Int), DataFormat.Binary);

                return position;
            }
        }
    }
}