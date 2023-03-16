using System;
using System.Collections.Generic;
using UnityEngine;

namespace JapeNet
{
    public partial class Request
    {
        public class Body
        {
            public virtual string ToJson() => JsonUtility.ToJson(this);
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
            public string database;

            public DatastoreBody(string command, string store) : base(command, Mode.Datastore)
            {
                database = store;
            }

            protected bool FindKey(string json, string key, out int valueIndex)
            {
                string matchKey = '"' + key + '"' + ':';
                valueIndex = json.IndexOf(matchKey, StringComparison.InvariantCulture);
                if (valueIndex >= 0)
                {
                    valueIndex += matchKey.Length;
                    return true;
                } 
                return false;
            }

            protected string TryUnwrapObject(string json, string key)
            {
                if (!FindKey(json, key, out int valueIndex))
                {
                    return json;
                }

                string startMatch = '"' + "{";
                string endMatch = "}" + '"';

                int startIndex = json.IndexOf(startMatch, valueIndex, StringComparison.InvariantCulture);
                int endIndex = json.IndexOf(endMatch, valueIndex, StringComparison.InvariantCulture);

                string wrapped = json.Substring(startIndex, endIndex - startIndex + 2);
                string unwrapped = wrapped.Replace(startMatch, "{").Replace(endMatch, "}").Replace(@"\" + '"', '"'.ToString());

                return json.Replace(wrapped, unwrapped);
            }

            public override string ToJson()
            {
                string json = base.ToJson();

                IEnumerator<string> unwrapFields = UnwrapFields();
                while (unwrapFields.MoveNext())
                {
                    json = TryUnwrapObject(json, unwrapFields.Current);
                }

                return json;
            }

            protected virtual IEnumerator<string> UnwrapFields() { yield return null; }
        }
    }
}
