namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class GetBody : DatastoreBody
            {
                public string collection;
                public string key;

                public GetBody(string store, string collection, string key) : base("Get", store)
                {
                    this.collection = collection;
                    this.key = key;
                }
            }
        }
    }
}