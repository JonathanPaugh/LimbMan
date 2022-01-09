using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class RemoveBody : DatacacheBody
            {
                public string id;

                public RemoveBody(string id) : base("remove")
                {
                    this.id = id;
                }
            }
        }
    }
}