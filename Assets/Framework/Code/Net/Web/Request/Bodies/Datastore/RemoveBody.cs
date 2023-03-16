namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class RemoveBody : DatastoreBody
            {
                public string collection;
                public string id;
                public string[] data;

                public RemoveBody(string store, string collection, string id, string[] data) : base("remove", store)
                {
                    this.collection = collection;
                    this.id = id;
                    this.data = data;
                }
            }
        }
    }
}