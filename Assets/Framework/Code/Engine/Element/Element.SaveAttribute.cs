using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.Serialization;
using UnityEngine;

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