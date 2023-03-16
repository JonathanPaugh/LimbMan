using System;

namespace Jape
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HidePickerAttribute : Attribute {}
}