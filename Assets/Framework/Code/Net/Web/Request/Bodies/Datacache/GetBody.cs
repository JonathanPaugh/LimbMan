namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class GetBody : DatacacheBody
            {
                public string key;

                public GetBody(string key) : base("Get")
                {
                    this.key = key;
                }
            }
        }
    }
}