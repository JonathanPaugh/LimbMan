using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class SerializerSettings : SettingsData
    {
        public Serializer.Mode mode;

        [PropertySpace(8)]

        public Serializer.BitConverter bitConverter;

        [PropertySpace(8)]

        public Serializer.StringEncoding stringEncoding;

        [PropertySpace(8)]

        [ShowIf(nameof(mode), Serializer.Mode.Dynamic)]
        public int writerLimit = 4;

        [ShowIf(nameof(mode), Serializer.Mode.Dynamic)]
        public int streamLimit = 2;
    }
}