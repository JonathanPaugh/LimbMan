using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

using Behaviour = Jape.Behaviour;

namespace JapeEditor
{
    public class DataViewWindow : ViewWindow
    {
        protected override string Title => "Data";

        protected override float Width => 768;
        protected override float MinHeight => 512;

        private DataRegionFlags dataRegion = DataRegionFlags.Game | DataRegionFlags.System;

        [BoxGroup]

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

        protected override Action<object> AddAction => delegate(object parent)
        {
            DataCreateWindow window = Open<DataCreateWindow>();

            Type type = (Type)parent;

            if (type.IsBaseOrSubclassOf(typeof(GameData))) { window.DataRegion = Jape.DataRegion.Game; }
            if (type.IsBaseOrSubclassOf(typeof(SystemData))) { window.DataRegion = Jape.DataRegion.System; }

            window.SetSelection(parent);
        };

        protected override string GetParentLabel(object parent) { return ((Type)parent).CleanName(); }
        protected override string GetChildLabel(object child) { return ((DataType)child).name; }

        protected override IEnumerable<object> ParentSelections()
        {
            List<Type> dataTypeClasses = new List<Type>();
            if (DataRegion.HasFlag(DataRegionFlags.Game)) { dataTypeClasses.AddRange(typeof(GameData).GetSubclass()); } 
            if (DataRegion.HasFlag(DataRegionFlags.System))
            {
                dataTypeClasses.AddRange(typeof(SystemData).
                                GetSubclass().
                                Where(d => !d.IsSubclassOf(typeof(SettingsData))).
                                Where(d => !d.IsBaseOrSubclassOf(typeof(Behaviour)))); 

            }
            return dataTypeClasses;
        }

        protected override IEnumerable<object> ChildSelections(object parent) { return DataType.FindAll((Type)parent); }
    }
}