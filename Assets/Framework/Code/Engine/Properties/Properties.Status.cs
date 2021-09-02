using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
	public partial class Properties
    {
        public class Status : Jape.Status
        {
            internal Vector3 position;

            protected override void OnSave() {}
            protected override void OnLoad() {}

            public override void Write(string key, object value)
            {
                Log.Warning("Cannot write to property status");
            }

            public override object Read(string key)
            {
                Log.Warning("Cannot read from property status");
                return null;
            }

            public override byte[] Serialize()
            {
                const int Size = Serializer.Bytes.Size.Float * 3;

                byte[] bytes = new byte[Size];

                for (int i = 0; i < Size; i += Serializer.Bytes.Size.Float)
                {
                    byte[] temp;
                    switch (i)
                    {
                        case 0:
                            temp = Serializer.Bytes.Get(position.x);
                            break;

                        case Serializer.Bytes.Size.Float:
                            temp = Serializer.Bytes.Get(position.y);
                            break;

                        default:
                            temp = Serializer.Bytes.Get(position.z);
                            break;
                    }
                    
                    for (int j = 0; j < temp.Length; j++)
                    {
                        bytes[i + j] = temp[j];
                    }
                }

                return bytes;
            }

            public override int Deserialize(byte[] data)
            {
                int position = 0;

                float x = Serializer.Bytes.ToFloat(data, position);
                position += Serializer.Bytes.Size.Float;

                float y = Serializer.Bytes.ToFloat(data, position);
                position += Serializer.Bytes.Size.Float;

                float z = Serializer.Bytes.ToFloat(data, position);
                position += Serializer.Bytes.Size.Float;

                this.position = new Vector3(x, y, z);

                return position;
            }
        }
    }
}