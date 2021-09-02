using System;
using Jape;

namespace Game
{
    [Serializable]
    public class Damage : Jape.Damage, Serializer.Dynamic.ISerializable
    {
        public float value;

        public byte[] Serialize()
        {
            return Serializer.Bytes.Get(value);
        }

        public int Deserialize(byte[] data, int position)
        {
            value = Serializer.Bytes.ToFloat(data, position);
            position += Serializer.Bytes.Size.Float;
            return position;
        }
    }
}