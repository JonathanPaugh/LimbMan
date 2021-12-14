using System.Collections.Generic;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class UpdateBody : DatastoreBody
            {
                public string collection;
                public string id;
                public string data;

                public UpdateBody(string store, string collection, string id, string data) : base("Update", store)
                {
                    this.collection = collection;
                    this.id = id;
                    this.data = data;
                }
            }
        }
    }
}