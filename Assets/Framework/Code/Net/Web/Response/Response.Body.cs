using UnityEngine;

namespace JapeNet
{
    public partial class Response
    {
        public class Body
        {
            public virtual string ToJson() => JsonUtility.ToJson(this);
        }
    }
}
