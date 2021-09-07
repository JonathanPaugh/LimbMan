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
            SaveManager.Save(null);
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
            SaveManager.Delete(null);
        }
    }
}