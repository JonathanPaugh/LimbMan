using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class SetBody : DatacacheBody
            {
                public string key;
                public string value;

                public SetBody(string key, string value) : base("Set")
                {
                    this.key = key;
                    this.value = value;
                }
            }
        }
    }
}