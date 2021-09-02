using Jape;

namespace JapeNet
{
    public class Packet : Pack
    {
        public Packet() {}

        public Packet(int id)
        {
            Write(id);
        }

        public Packet(byte[] data) : base(data) {}
    }
}
