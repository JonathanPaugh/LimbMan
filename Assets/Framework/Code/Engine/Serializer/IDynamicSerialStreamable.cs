namespace Jape
{
    public static partial class Serializer
    {
        public static partial class Dynamic
        {
            public interface ISerialStreamable
            {
                void SerialStream(SerialStream stream);
            }
        }
    }
}