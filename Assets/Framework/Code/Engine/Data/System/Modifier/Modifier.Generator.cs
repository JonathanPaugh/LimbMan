using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public partial class Modifier
    {
        [Serializable]
        public new class Generator<T> : Behaviour.Generator<ModifierInstance<T>> where T : Element
        {                                        
            [PropertyOrder(1)]
            [ShowIf(nameof(IsActive))]
            [SerializeField] 
            internal float lifetime = -1;

            protected override BehaviourType behaviourType => FindAll<ModifierType>().FirstOrDefault(m => m.TargetType == typeof(T));

            protected override object GetBehaviours() { return Dropdown<Modifier>(); }

            protected override List<string> GetArgs() { return !IsActive() ? null : BehaviourInstance.DeclaredArgs(behaviour.Type).ToList(); }

            public ModifierInstance<T> InflictInstance(T target, T inflictor = null) { return ModifierInstance<T>.Inflict((Modifier)behaviour, target, inflictor, lifetime, args); }
        }
    }
}