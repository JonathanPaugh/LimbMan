using System;
using UnityEngine;

namespace Jape
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class PickFilterAttribute : Attribute
    {
        public string MethodName { get; }

        public PickFilterAttribute(string methodName) { MethodName = methodName; }
    }
}