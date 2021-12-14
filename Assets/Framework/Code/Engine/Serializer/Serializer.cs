using System;

namespace Jape
{
	public static partial class Serializer
    {
        public enum Mode { Simple, Dynamic, Odin };

        public enum BitConverter { Default }
        public enum StringEncoding { Ascii, Utf8, Utf32 }

        private static SerializerSettings settings;
        public static SerializerSettings Settings => settings ?? (settings = Game.Settings<SerializerSettings>());

        internal static void Init()
        {
            settings = Game.Settings<SerializerSettings>();
        }

        public static byte[] Serialize<T>(T value)
        {
            switch (Settings.mode)
            {
                case Mode.Simple: return Simple.Serialize(value);  
                case Mode.Dynamic: return Dynamic.Serialize(value);  
                case Mode.Odin: return Odin.Serialize(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] SerializeWeak(object value)
        {
            switch (Settings.mode)
            {
                case Mode.Simple: return Simple.SerializeWeak(value);  
                case Mode.Dynamic: return Dynamic.SerializeWeak(value);  
                case Mode.Odin: return Odin.SerializeWeak(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            switch (Settings.mode)
            {
                case Mode.Simple: return Simple.Deserialize<T>(data);
                case Mode.Dynamic: return Dynamic.Deserialize<T>(data);
                case Mode.Odin: return Odin.Deserialize<T>(data);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static object DeserializeWeak(byte[] data)
        {
            switch (Settings.mode)
            {
                case Mode.Simple: return Simple.DeserializeWeak(data);
                case Mode.Dynamic: return Dynamic.DeserializeWeak(data);
                case Mode.Odin: return Odin.DeserializeWeak(data);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal static int DeserializeInternal(byte[] data, out object value, int position)
        {
            switch (Settings.mode)
            {
                case Mode.Simple: return Simple.DeserializeInternal(data, out value, position);
                case Mode.Dynamic: return Dynamic.DeserializeInternal(data, out value, position);
                case Mode.Odin: return Odin.DeserializeInternal(data, out value, position);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}