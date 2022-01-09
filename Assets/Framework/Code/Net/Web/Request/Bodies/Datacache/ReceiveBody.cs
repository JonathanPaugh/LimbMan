using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class ReceiveBody : DatacacheBody
            {
                public string subscription;

                public ReceiveBody(string subscription) : base("receive")
                {
                    this.subscription = subscription;
                }
            }
        }
    }
}