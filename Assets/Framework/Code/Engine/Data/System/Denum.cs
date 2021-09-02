using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class Denum : SystemData
    {
        [Flags] public enum FlagsMode { Default = 1, Flags = 2 }

        protected new static string Path => "System/Resources/Denums";

        private static string ScriptPath => "Denums";

        [ShowInInspector]
        [PropertyOrder(-1)]
        [EnableIf(Game.GameDeveloperMode)]
        [EnumToggleButtons]
        public Sector Sector
        {
            get => sector;
            set
            {
                if (sector == value) { return; }
                sector = value;
                Erase();
                Write();
            }
        }

        [SerializeField, HideInInspector]
        private Sector sector = Sector.Game;

        [PropertySpace(8)]

        [ShowInInspector]
        [PropertyOrder(-1)]
        [EnumToggleButtons]
        public FlagsMode Mode
        {
            get => mode;
            set
            {
                if (mode == value) { return; }
                mode = value;
                Write();
            }
        }

        [SerializeField, HideInInspector]
        private FlagsMode mode = FlagsMode.Default | FlagsMode.Flags;

        [PropertySpace(8)]

        [ShowInInspector]
        [PropertyOrder(-1)]
        public bool UseNone
        {
            get => useNone;
            set
            {
                if (useNone == value) { return; }
                useNone = value;
                Write();
            }
        }

        [SerializeField, HideInInspector]
        private bool useNone;

        [Space(16)]

        [SerializeField]
        [HideLabel]
        [ReadOnly]
        public TextAsset script;

        [Space(8)]

        [NonSerialized, OdinSerialize]
        [ListDrawerSettings(ShowPaging = false)]
        public ObservableCollection<string> entries = new ObservableCollection<string>();

        [PropertySpace(16)]

        [Button(ButtonSizes.Large)]
        private void DeleteSelf()
        {
            Erase();
            DestroyAsset();
        }

        private string storedName = string.Empty;

        private bool IsDefault() { return Mode.HasFlag(FlagsMode.Default); }
        private bool IsFlags() { return Mode.HasFlag(FlagsMode.Flags); }

        private bool IsNewName() { return !string.IsNullOrEmpty(storedName) && storedName != name; }

        private string GetPath() { return Framework.GetPath(sector); }

        protected override void EnabledEditor()
        {
            entries.CollectionChanged += QueueRewrite;
        }

        protected override void DisabledEditor()
        {
            entries.CollectionChanged -= QueueRewrite;
        }

        private void QueueRewrite(object sender, NotifyCollectionChangedEventArgs e)
        {
            DenumManager.QueueRewrite(this);
        }

        private void Change()
        {
            Erase();
            storedName = name;
        }

        public void Rewrite()
        {
            #if UNITY_EDITOR
            if (entries == null) { return; }
            if (entries.Count == 0) { Erase(); }
            Write();
            #endif
        }

        private void Write()
        {
            #if UNITY_EDITOR

            if (IsNewName()) { Change(); }

            Script template = Database.GetAsset<Script>(nameof(Denum)).Load<Script>();

            template.Inject = Inject;

            script = template.CreateAsset(sector, CodeRegion.Engine, name);

            string Inject(int index)
            {
                switch (index) {
                    case 0:
                    {
                        if (!IsDefault()) { return string.Empty; }
                        IEnumerable<string> strings = entries.Select(e => $"{e} = {entries.IndexOf(e) + 1}");
                        if (useNone) { strings = strings.Prepend("None = 0"); }
                        return string.Join($", {Environment.NewLine}", strings);
                    }
                        

                    case 1:
                    {
                        if (!IsFlags()) { return string.Empty; }
                        IEnumerable<string> strings = entries.Select(e => $"{e} = {Mathf.Pow(2, entries.IndexOf(e))}");
                        if (useNone) { strings = strings.Prepend("None = 0"); }
                        return string.Join($", {Environment.NewLine}", strings);
                    }

                    default: return null;
                }
            }

            #endif
        }

        private void Erase()
        {
            #if UNITY_EDITOR
            if (script == null) { return; }

            Directory.Delete(script);
            #endif
        }
    }
}
