using System;
using System.Linq;
using System.Text;

namespace Jape
{
    public abstract partial class Mono
    {
        public class Key
        {
            private const string IdentifierSeperator = "-";
            private const string EncodedSeperator = "";

            public enum IdentifierEncoding { Ascii, Base64, Hex }
            private readonly Encodable encodable;

            public Key(Type type, string identifier, IdentifierEncoding identifierEncoding)
            {
                encodable = new Encodable(
                    new Encodable.Segment(type.FullName, Encoding.ASCII.GetBytes),
                    new Encodable.Segment(identifier, value => EncodeIdentifier(value, identifierEncoding))
                );
            }

            private static byte[] EncodeIdentifier(string key, IdentifierEncoding identifierEncoding)
            {
                key = key.Replace(IdentifierSeperator, EncodedSeperator);
                switch (identifierEncoding)
                {
                    default: return Encoding.ASCII.GetBytes(key);
                    case IdentifierEncoding.Base64: return Convert.FromBase64String(key);
                    case IdentifierEncoding.Hex: return EncodeHex(key);
                }
            }

            private static string DecodeIdentifier(byte[] bytes, IdentifierEncoding identifierEncoding)
            {
                switch (identifierEncoding)
                {
                    default: return Encoding.ASCII.GetString(bytes);
                    case IdentifierEncoding.Base64: return Convert.ToBase64String(bytes);
                    case IdentifierEncoding.Hex: return DecodeHex(bytes);
                }
            }

            private static byte[] EncodeHex(string key)
            {
                const int DigitsPerByte = 2;

                byte[] bytes = new byte[key.Length / DigitsPerByte];
                for (int i = 0; i < key.Length; i += DigitsPerByte)
                {
                    bytes[i / DigitsPerByte] = Convert.ToByte(key.Substring(i, DigitsPerByte), 16);
                }
                return bytes;
            }

            private static string DecodeHex(byte[] bytes)
            {
                return BitConverter.ToString(bytes).ToLowerInvariant().Replace(IdentifierSeperator, EncodedSeperator);
            }

            internal static string DecodePairKey(byte[] bytes, int identifierLength, IdentifierEncoding identifierEncoding)
            {
                byte[] typeBytes = bytes.Take(bytes.Length - identifierLength).ToArray();
                byte[] identifierBytes = bytes.Skip(bytes.Length - identifierLength).ToArray();
                
                return $"{Encoding.ASCII.GetString(typeBytes)}{Encodable.Seperator}{DecodeIdentifier(identifierBytes, identifierEncoding)}";
            }

            public byte[] Encode() => encodable.Encode();
            public bool Compare(byte[] encodedKey) => Encode().SequenceEqual(encodedKey);
            public override string ToString() => encodable.ToString();
        }
    }
}