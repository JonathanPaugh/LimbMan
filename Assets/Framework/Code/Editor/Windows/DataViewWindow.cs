using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using Behaviour = Jape.Behaviour;

namespace JapeEditor
{
    public class DataViewWindow : ViewWindow<Type, DataType>
    {
        protected override string Title => "Data";

        protected override float Width => 768;
        protected override float MinHeight => 512;

        private PropertyTree targetTree;
        private DataRegionFlags dataRegion = DataRegionFlags.Game | DataRegionFlags.System;

        [ShowInInspector]
        [HideLabel]
        [EnumToggleButtons]
        public DataRegionFlags DataRegion
        {
            get => dataRegion;
            set
            {
                if (dataRegion != value) { ResetSelector(); }
                dataRegion = value;
            } 
        }

        protected override Action<Type, string> AddAction => delegate(Type group, string _)
        {
            DataCreateWindow window = Open<DataCreateWindow>();

            if (group.IsBaseOrSubclassOf(typeof(GameData))) { window.DataRegion = Jape.DataRegion.Game; }
            if (group.IsBaseOrSubclassOf(typeof(SystemData))) { window.DataRegion = Jape.DataRegion.System; }

            window.SetSelection(group);
        };

        protected override string GetGroupLabel(Type group) { return group.CleanName(); }
        protected override string GetTargetLabel(DataType target) { return target.name; }

        protected override IEnumerable<Type> GroupSelections()
        {
            List<Type> dataTypeClasses = new();
            if (DataRegion.HasFlag(DataRegionFlags.Game)) { dataTypeClasses.AddRange(typeof(GameData).GetSubclass()); } 
            if (DataRegion.HasFlag(DataRegionFlags.System))
            {
                dataTypeClasses.AddRange(typeof(SystemData)
                               .GetSubclass()
                               .Where(d => !d.IsSubclassOf(typeof(SettingsData)))
                               .Where(d => !d.IsBaseOrSubclassOf(typeof(Behaviour)))); 

            }
            return dataTypeClasses;
        }

        protected override IEnumerable<DataType> TargetSelections(Type parent) { return DataType.FindAll(parent); }

        protected override void DrawTarget()
        {
            DrawInspectButton(Target);
            targetTree?.Draw();
        }

        protected override void OnSelectTarget(DataType target)
        {
            targetTree = PropertyTree.Create(target);
        }
    }
}