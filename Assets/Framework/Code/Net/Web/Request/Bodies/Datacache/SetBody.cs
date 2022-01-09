using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class SetBody : DatacacheBody
            {
                public string id;
                public string value;

                public SetBody(string id, string value) : base("set")
                {
                    this.id = id;
                    this.value = value;
                }
            }
        }
    }
}