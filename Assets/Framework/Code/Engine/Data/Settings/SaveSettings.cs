using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class SaveSettings : SettingsData
    {
        [PropertyOrder(-1)]
        public bool save;

        [PropertySpace(0, 8)]

        [PropertyOrder(-1)]
        public bool load;

        [HorizontalGroup("Interval")]

        [PropertyOrder(-1)]
        [LabelText("Save Interval")]
        public bool autoSave;

        [HorizontalGroup("Interval")]

        [PropertyOrder(-1)]
        [ShowIf(nameof(autoSave))]
        [DisableIf(Game.GameIsRunning)]
        [HideLabel]
        [SuffixLabel("Seconds", true)]
        public uint interval;

        [HorizontalGroup("Interval")]

        [PropertyOrder(-1)]
        [ShowIf(nameof(autoSave))]
        [LabelWidth(32)]
        [Tooltip("Does not request data before auto save")]
        public bool light;

        [SerializeField, HideInInspector]
        private int profile;

        [PropertySpace(8)]

        [ShowInInspector]
        [DisableIf(Game.GameIsRunning)]
        public int Profile
        {
            get => profile;
            set => profile = value;
        }

        [HorizontalGroup("Buttons")]

        [ShowInInspector]
        [EnableIf(Game.GameIsRunning)]
        [LabelText("Save")]
        private void SaveProfile()
        {
            SaveManager.Save();
        }

        [HorizontalGroup("Buttons")]

        [ShowInInspector]
        [EnableIf(Game.GameIsRunning)]
        [LabelText("Load")]
        private void LoadProfile()
        {
            SaveManager.Load(null);
        }

        [HorizontalGroup("Buttons")]

        [ShowInInspector]
        [DisableIf(Game.GameIsRunning)]
        [LabelText("Delete")]
        private void DeleteProfile()
        {
            SaveManager.Delete();
        }
    }
}