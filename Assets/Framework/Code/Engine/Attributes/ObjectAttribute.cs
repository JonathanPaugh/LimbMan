using System;
using System.Collections.Generic;

namespace Jape
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ObjectAttribute : Attribute
    {
        internal List<string> MethodNames = new();
        internal bool HidePicker = false;
        internal int PickerMode = 0;
    }
}