using System.Collections.Generic;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datastore
        {
            public class InsertBody : DatastoreBody
            {
                public string collection;
                public string data;

                public InsertBody(string store, string collection, string data) : base("Insert", store)
                {
                    this.collection = collection;
                    this.data = data;
                }
            }
        }
    }
}