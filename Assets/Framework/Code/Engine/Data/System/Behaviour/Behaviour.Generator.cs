using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Jape
{
    public partial class Behaviour
    {
        [Serializable]
        public class Generator<T> where T : BehaviourInstance
        {
            private const Sector Sector = Jape.Sector.Game;

            protected string BehaviourLabel => $"{behaviourType.Type.CleanName()}";
            protected string ArgsLabel => $"{behaviour.name} Args";

            protected virtual BehaviourType behaviourType => FindAll<BehaviourType>().FirstOrDefault(b => b.Type == typeof(T));

            [PropertyOrder(-1)]
            [SerializeField]
            [ShowIf(nameof(Exists))]
            [LabelText("$" + nameof(BehaviourLabel))]
            [ValueDropdown(nameof(GetBehaviours))]
            internal Behaviour behaviour;
            
            [PropertySpace(8, 16)]

            [PropertyOrder(0)]
            [SerializeField, ShowInInspector]
            [ShowIf(nameof(IsActive))]
            internal List<Arg> args = new();

            [ShowIf(nameof(Exists))]
            [HideIf(nameof(IsSet))]
            [Button(ButtonSizes.Medium)]
            [LabelText("Create")]
            protected void CreateEditor()
            {
                #if UNITY_EDITOR
                Object selection = UnityEditor.Selection.activeObject;
                InputWindow.Call(input =>
                {
                    behaviour = behaviourType.CreateBehaviourEditor(input, Sector);
                    behaviour.SaveEditor();
                    UnityEditor.Selection.activeObject = selection;
                }, "Create Behaviour", "Create");
                #endif
            }

            [PropertySpace(SpaceAfter = 16)]

            [ShowInInspector]
            [ShowIf(nameof(IsSet))]
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden, Expanded = true)]
            private Behaviour selection
            {
                get => behaviour;
                set => _ = value;
            }

            [PropertyOrder(2)]
            [ShowInInspector, ReadOnly]
            [ShowIf(nameof(HasArgs))]
            [LabelText("$" + nameof(ArgsLabel))]
            [ListDrawerSettings(ShowItemCount = false, Expanded = true)]
            private List<string> displayArgs => GetArgs();

            public bool Exists() { return behaviourType != null; }
            public bool IsSet() { return behaviour != null; }
            public bool IsActive() { return IsSet() && behaviour.IsActive(); }

            private bool HasArgs()
            {
                if (GetArgs() == null) { return false; }
                return GetArgs().Count > 0;
            }

            protected virtual object GetBehaviours() { return Dropdown<Behaviour>(); }

            protected virtual List<string> GetArgs() { return !IsActive() ? null : BehaviourInstance.DeclaredArgs(behaviour.Type).ToList(); }
            
            public T CreateInstance() { return (T)BehaviourInstance.Create(behaviour, args); }
        }
    }
}