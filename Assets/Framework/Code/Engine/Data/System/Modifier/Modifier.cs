using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Jape
{
    public partial class Modifier : Behaviour
    {
        protected new static string Path => "System/Resources/Modifiers";

        protected override string NamePrefix() { return $"_{base.NamePrefix()}"; }

        public new static Modifier Find<T>() where T : ModifierInstance { return FindAll<Modifier>().FirstOrDefault(m => m.Type == typeof(T)); }
    }
}