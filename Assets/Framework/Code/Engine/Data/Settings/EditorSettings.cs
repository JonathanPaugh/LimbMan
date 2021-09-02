using UnityEngine;

namespace Jape
{
    public class EditorSettings : SettingsData
    {
        public bool autoProperty = false;

        [Space(10)]

        public float processingRate = 0.2f;
    }
}