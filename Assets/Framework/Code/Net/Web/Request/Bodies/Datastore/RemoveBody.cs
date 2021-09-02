using System.Collections.Generic;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class RemoveBody : DatastoreBody
            {
                public string collection;
                public string key;
                public string[] data;

                public RemoveBody(string store, string collection, string key, string[] data) : base("Remove", store)
                {
                    this.collection = collection;
                    this.key = key;
                    this.data = data;
                }
            }
        }
    }
}