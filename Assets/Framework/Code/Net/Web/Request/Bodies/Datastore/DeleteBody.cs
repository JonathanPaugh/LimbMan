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
                public string key;
                public string data;

                public UpdateBody(string store, string collection, string key, string data) : base("Update", store)
                {
                    this.collection = collection;
                    this.key = key;
                    this.data = data;
                }
            }
        }
    }
}