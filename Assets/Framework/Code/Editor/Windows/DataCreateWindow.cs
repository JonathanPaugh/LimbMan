using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using Jape;

namespace JapeEditor
{
    public class DataCreateWindow : CreateWindow
    {
        protected override string Title => "Create Data";

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;
        protected override float Width => 256;

        private bool ForSystem => DataRegion == DataRegion.System;

        protected override Action<object> CreateAction => delegate(object selection)
        {
            switch (DataRegion)
            {
                case DataRegion.Game: default:
                    DataType.CreateData((Type)selection, null, null, true);
                    break;

                case DataRegion.System:
                    DataType.CreateData((Type)selection, SystemData.GetPath((Type)selection, sector), null, true);
                    break;
            }


        };

        private DataRegion dataRegion;

        [PropertyOrder(-2)]
        [EnumToggleButtons]
        [ShowInInspector, HideLabel]
        public DataRegion DataRegion 
        { 
            get => dataRegion;
            set
            {
                if (value == DataRegion.Game) { sector = Sector.Game; }
                ResetSelection();
                dataRegion = value;
            }
        }

        [PropertyOrder(-1)]
        [EnableIf(Framework.FrameworkDeveloperMode)]
        [ShowIf(nameof(ForSystem))]
        [HideLabel]
        [EnumToggleButtons]
        public Sector sector = Sector.Game;

        [ShowInInspector]
        [PropertyOrder(3)]
        [ShowIf(nameof(Path))]
        [HideLabel, ReadOnly]
        public string Path
        {
            get
            {
                if (!IsSet()) { return null; }
                return ForSystem ? SystemData.GetPath((Type)Selection, sector) : $"Selection: {IO.Editor.SelectionFolder}";
            }   
        }

        protected override IList<object> Selections()
        {
            switch (dataRegion)
            {
                case DataRegion.Game: return GetGameDataTypes().Cast<object>().ToList();
                case DataRegion.System: return GetSystemDataTypes().Cast<object>().ToList();
                default: return null;
            }
        }

        protected IEnumerable<Type> GetGameDataTypes() { return typeof(GameData).GetSubclass(); }
        protected IEnumerable<Type> GetSystemDataTypes() { return typeof(SystemData).GetSubclass().Where(d => !d.IsBaseOrSubclassOf(typeof(Behaviour))); }

        [MenuItem("Assets/Create/Data", false, -10)]
        private static void Menu() { Open<DataCreateWindow>(); }
    }
}