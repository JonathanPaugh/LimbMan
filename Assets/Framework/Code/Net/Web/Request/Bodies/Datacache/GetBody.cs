namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class GetBody : DatacacheBody
            {
                public string id;

                public GetBody(string id) : base("Get")
                {
                    this.id = id;
                }
            }
        }
    }
}