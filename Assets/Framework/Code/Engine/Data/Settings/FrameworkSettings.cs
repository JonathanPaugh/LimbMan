using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class FrameworkSettings : SettingsData 
    {
        public const string EngineSuffix = "";
        public const string EditorSuffix = "Editor";
        public const string NetSuffix = "Net";

        public const string EnginePath = "Code/Engine";
        public const string EditorPath = "Code/Editor";
        public const string NetPath = "Code/Net";

        public bool developerMode;

        [TabGroup("Tabs", "Framework")]
        public string frameworkPath = "Assets/Framework";

        [TabGroup("Tabs", "Framework")]
        public string frameworkNamespace = "Jape";

        [PropertySpace(8)]

        [TabGroup("Tabs", "Framework")]
        [HideReferenceObjectPicker]
        [Eject]
        public Assembly frameworkEngine;

        [PropertySpace(8)]

        [TabGroup("Tabs", "Framework")]
        [HideReferenceObjectPicker]
        [Eject]
        public Assembly frameworkEditor;

        [PropertySpace(8)]

        [TabGroup("Tabs", "Framework")]
        [HideReferenceObjectPicker]
        public Assembly[] frameworkAssemblies = new Assembly[0];

        [TabGroup("Tabs", "Game")]
        public string gamePath = "Assets/Game";

        [TabGroup("Tabs", "Game")]
        public string gameNamespace = "Game";

        [PropertySpace(8)]

        [TabGroup("Tabs", "Game")]
        [HideReferenceObjectPicker]
        [Eject]
        public Assembly gameEngine;

        [PropertySpace(8)]

        [TabGroup("Tabs", "Game")]
        [HideReferenceObjectPicker]
        [Eject]
        public Assembly gameEditor;

        [PropertySpace(8)]

        [TabGroup("Tabs", "Game")]
        [HideReferenceObjectPicker]
        public Assembly[] gameAssemblies = new Assembly[0];

        [Serializable]
        public class Assembly
        {
            [SerializeField, HideInInspector]
            private TextAsset reference;

            [PropertyOrder(1)]
            [HidePicker]
            [ShowInInspector]
            private TextAsset Reference
            {
                get { return reference; }
                set
                {
                    name = value.name;
                    reference = value;
                }
            }

            [PropertyOrder(2)]
            [ReadOnly]
            public string name;
        }
    }
}