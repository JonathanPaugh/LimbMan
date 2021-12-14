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
                key = NetManager.Settings.MasterKey;
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

            public string key;
            public byte index;
            public string command;

            protected DatabaseBody(string command, Mode mode)
            {
                key = NetManager.Settings.DatabaseKey;
                this.command = command;
                index = (byte)mode;
            }
        }

        public class DatacacheBody : DatabaseBody
        {
            public DatacacheBody(string command) : base(command, Mode.Datacache) {}
        }

        public class DatastoreBody : DatabaseBody
        {
            public string store;

            public DatastoreBody(string command, string store) : base(command, Mode.Datastore)
            {
                this.store = store;
            }
        }
    }
}
