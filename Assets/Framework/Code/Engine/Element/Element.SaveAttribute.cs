using System;

namespace Jape
{
    public abstract partial class Element
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class SaveAttribute : Attribute
        {
            internal string Key { get; }
            public SaveAttribute(string key = null) { Key = key; }
        }
    }
}