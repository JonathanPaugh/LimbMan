namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class GetBody : DatastoreBody
            {
                public string collection;
                public string id;

                public GetBody(string store, string collection, string id) : base("get", store)
                {
                    this.collection = collection;
                    this.id = id;
                }
            }
        }
    }
}