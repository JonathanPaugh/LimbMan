using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class FrameworkSettings : SettingsData 
    {
        public const string EngineSuffix = "";
        public const string EditorSuffix = "Editor";
        public const string NetSuffix = "Net";

        public static string EnginePath = IO.JoinPath("Code", "Engine");
        public static string EditorPath = IO.JoinPath("Code", "Editor");
        public static string NetPath = IO.JoinPath("Code", "Net");

        [TabGroup("Tabs", "Framework")]
        public string frameworkPath = IO.JoinPath("Assets", "Framework");

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
        public Assembly[] frameworkAssemblies = Array.Empty<Assembly>();

        [TabGroup("Tabs", "Game")]
        public string gamePath = IO.JoinPath("Assets", "Game");

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
        public Assembly[] gameAssemblies = Array.Empty<Assembly>();

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