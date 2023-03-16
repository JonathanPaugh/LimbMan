using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace JapeEditor
{
    public class BehaviourViewWindow : ViewWindow<BehaviourType, Behaviour>
    {
        protected override string Title => "Behaviours";

        protected override bool AddInput => true;

        private PropertyTree targetTree;
        private BehaviourRegion behaviourRegion = BehaviourRegion.Behaviour;

        [ShowInInspector]
        [HideLabel]
        [EnumToggleButtons]
        public BehaviourRegion BehaviourRegion
        {
            get => behaviourRegion;
            set
            {
                if (behaviourRegion != value) { ResetSelector(); }
                behaviourRegion = value;
            } 
        }

        protected override Action<BehaviourType, string> AddAction => delegate(BehaviourType parent, string input)
        {
            parent.BehaviourName = input;
            parent.CreateBehaviourEditor();
        };

        protected override IEnumerable<BehaviourType> GroupSelections()
        {
            switch (behaviourRegion)
            {
                case BehaviourRegion.Behaviour: return DataType.FindAll<BehaviourType>().Where(b => b.GetType() == typeof(BehaviourType));
                case BehaviourRegion.Modifier: return DataType.FindAll<ModifierType>().Where(b => b.GetType() == typeof(ModifierType));
                default: return null;
            }
        }

        protected override IEnumerable<Behaviour> TargetSelections(BehaviourType parent)
        {
            return parent.Behaviours;
        }

        protected override string GetGroupLabel(BehaviourType group) { return group.name; }
        protected override string GetTargetLabel(Behaviour target) { return target.name; }

        protected override void DrawTarget()
        {
            DrawInspectButton(Target);
            targetTree?.Draw();
        }

        protected override void OnSelectTarget(Behaviour target)
        {
            targetTree = PropertyTree.Create(target);
        }
    }
}