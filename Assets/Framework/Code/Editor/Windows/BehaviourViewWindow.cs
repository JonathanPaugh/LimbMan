using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.OdinInspector;

namespace JapeEditor
{
    public class BehaviourViewWindow : ViewWindow
    {
        protected override string Title => "Behaviours";

        protected override bool AddInput => true;

        private BehaviourRegion behaviourRegion = BehaviourRegion.Behaviour;

        [BoxGroup]

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

        protected override Action<object> AddAction => delegate(object parent)
        {
            BehaviourType behaviourType = (BehaviourType)parent;
            behaviourType.BehaviourName = Input;
            behaviourType.CreateBehaviour();
        };

        protected override IEnumerable<object> ParentSelections()
        {
            switch (behaviourRegion)
            {
                case BehaviourRegion.Behaviour: return DataType.FindAll<BehaviourType>().Where(b => b.GetType() == typeof(BehaviourType));
                case BehaviourRegion.Modifier: return DataType.FindAll<ModifierType>().Where(b => b.GetType() == typeof(ModifierType));
                default: return null;
            }
        }

        protected override IEnumerable<object> ChildSelections(object parent)
        {
            BehaviourType behaviourType = (BehaviourType)parent;
            return behaviourType.Behaviours;
        }

        protected override string GetParentLabel(object parent) { return ((BehaviourType)parent).name; }
        protected override string GetChildLabel(object child) { return ((Behaviour)child).name; }
    }
}