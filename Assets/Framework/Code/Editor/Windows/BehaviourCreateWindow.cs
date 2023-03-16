using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.OdinInspector;

namespace JapeEditor
{
    public class BehaviourCreateWindow : CreateWindow
    {

        protected override string Title => "Create Behaviour";

        [ShowInInspector]
        [PropertyOrder(-1)]
        [HideLabel]
        [EnumToggleButtons]
        protected BehaviourRegion BehaviourRegion
        {
            get => behaviourRegion;
            set
            {
                if (behaviourRegion != value) { ResetSelection(); }
                behaviourRegion = value;
            }
        }

        private BehaviourRegion behaviourRegion;

        protected override IList<object> Selections()
        {
            switch (behaviourRegion)
            {
                case BehaviourRegion.Behaviour: return DataType.DropdownStrict<BehaviourType>().Cast<object>().ToArray();
                case BehaviourRegion.Modifier: return DataType.DropdownStrict<ModifierType>().Cast<object>().ToList();
                default: return default;
            }
        }

        protected override Action<object> CreateAction => delegate(object selection)
        {
            ((BehaviourType)selection).CreateBehaviourEditor();
        };
    }
}