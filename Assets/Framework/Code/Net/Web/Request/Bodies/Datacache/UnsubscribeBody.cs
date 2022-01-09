using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class UnsubscribeBody : DatacacheBody
            {
                public string subscription;

                public UnsubscribeBody(string subscription) : base("unsubscribe")
                {
                    this.subscription = subscription;
                }
            }
        }
    }
}