using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class SubscribeBody : DatacacheBody
            {
                public new enum Mode { Sequential, Concurrent }

                public string channel;
                public byte mode;

                public SubscribeBody(string channel, Mode mode) : base("subscribe")
                {
                    this.channel = channel;
                    this.mode = (byte)mode;
                }
            }
        }
    }
}