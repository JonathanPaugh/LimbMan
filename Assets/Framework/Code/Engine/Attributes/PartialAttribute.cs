using System;
using UnityEngine;

namespace Jape
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PartialAttribute : Attribute
    {
        public string Selector { get; set; }

        public PartialAttribute(string selector) { Selector = selector; }
    }
}