using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class RemoveBody : DatacacheBody
            {
                public string key;

                public RemoveBody(string key) : base("Remove")
                {
                    this.key = key;
                }
            }
        }
    }
}