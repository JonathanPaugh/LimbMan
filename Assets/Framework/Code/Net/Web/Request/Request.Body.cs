using System;
using System.IO;
using System.Net;
using System.Text;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public class Body
        {
            public string ToJson() => JsonUtility.ToJson(this);
        }

        public class ServerBody : Body
        {
            public string key;
            public string id;

            public ServerBody(string id)
            {
                key = NetManager.Settings.MasterServerKey();
                this.id = id;
            }
        }

        public class ClientBody : Body
        {
            public string id;

            public ClientBody(string id)
            {
                this.id = id;
            }
        }

        public abstract class DatabaseBody : Body
        {
            public enum Mode { Datastore, Datacache }

            public string id;
            public byte index;

            protected DatabaseBody(string id, Mode mode)
            {
                this.id = id;
                index = (byte)mode;
            }
        }

        public class DatacacheBody : DatabaseBody
        {
            public DatacacheBody(string id) : base(id, Mode.Datacache) {}
        }

        public class DatastoreBody : DatabaseBody
        {
            public string store;

            public DatastoreBody(string id, string store) : base(id, Mode.Datastore)
            {
                this.store = store;
            }
        }
    }
}
