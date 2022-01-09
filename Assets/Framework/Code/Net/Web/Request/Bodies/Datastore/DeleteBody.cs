using System.Collections.Generic;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class DeleteBody : DatastoreBody
            {
                public string collection;
                public string id;

                public DeleteBody(string store, string collection, string id) : base("delete", store)
                {
                    this.collection = collection;
                    this.id = id;
                }
            }
        }
    }
}