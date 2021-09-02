using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public abstract partial class Element
    {
        public class Status : Jape.Status
        {
            private Dictionary<string, object> attributeData;
            internal Dictionary<string, object> AttributeData => attributeData ?? (attributeData = new Dictionary<string, object>());

            public override byte[] Serialize()
            {
                return Serializer.Bytes.Combine
                (
                    base.Serialize(), 
                    Serializer.Dynamic.WriteDictionary(AttributeData)
                );
            }

            public override int Deserialize(byte[] data)
            {
                int position = base.Deserialize(data);

                position = Serializer.Dynamic.ReadDictionary(data, out Dictionary<string, object> dictionary, position);
                attributeData = dictionary;

                return position;
            }
        }
    }
}