using UnityEngine;

namespace Jape
{
    public static partial class Serializer
    {
        public static partial class Dynamic
        {
            public interface ISerializable
            {
                byte[] Serialize();
                int Deserialize(byte[] data, int position);
            }
        }
    }

}