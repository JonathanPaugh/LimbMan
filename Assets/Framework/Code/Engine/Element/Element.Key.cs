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

            public enum IdentifierEncoding { ASCII, Base64, Hex }
            private readonly Encodable encodable;

            public Key(Type type, string identifier, IdentifierEncoding identifierEncoding)
            {
                encodable = new Encodable
                (
                    new Encodable.Segment(type.FullName, Encoding.ASCII.GetBytes),
                    new Encodable.Segment(identifier, EncodeIdentifier)
                );

                byte[] EncodeIdentifier(string key)
                {
                    key = key.Replace(IdentifierSeperator, EncodedSeperator);
                    switch (identifierEncoding)
                    {
                        default: return Encoding.ASCII.GetBytes(key);
                        case IdentifierEncoding.Base64: return Convert.FromBase64String(key);
                        case IdentifierEncoding.Hex: return FromHexString(key);
                    }
                }
            }

            public byte[] FromHexString(string key)
            {
                const int DigitsPerByte = 2;

                byte[] bytes = new byte[key.Length / DigitsPerByte];
                for (int i = 0; i < key.Length; i += DigitsPerByte)
                {
                    bytes[i / DigitsPerByte] = Convert.ToByte(key.Substring(i, DigitsPerByte), 16);
                }
                return bytes;
            }

            public byte[] Encode() => encodable.Encode();
            public bool Compare(byte[] encodedKey) => Encode().SequenceEqual(encodedKey);
            public override string ToString() => encodable.ToString();
        }
    }
}