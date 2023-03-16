using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class EditorSettings : SettingsData
    {
        [SerializeField, HideInInspector]
        private bool developerMode;
        
        [ShowInInspector]
        public bool DeveloperMode
        {
            get => developerMode;
            set
            {
                developerMode = value;
                SaveEditor();
            }
        }

        [SerializeField, HideInInspector]
        private bool exposePackages;

        [ShowInInspector]
        public bool ExposePackages
        {
            get => exposePackages;
            set
            {
                exposePackages = value;
                SaveEditor();
                OnExposePackages.Invoke(exposePackages);
            }
        }

        [NonSerialized]
        public Action<bool> OnExposePackages = delegate {};
    }
}