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
                public string key;

                public DeleteBody(string store, string collection, string key) : base("Delete", store)
                {
                    this.collection = collection;
                    this.key = key;
                }
            }
        }
    }
}