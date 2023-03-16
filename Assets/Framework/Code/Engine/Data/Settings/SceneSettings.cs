using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class SceneSettings : SettingsData
    {
        [SerializeField]
        public float sceneFade = 1;

        [Space(8)]

        public Map baseMap;

        [Space(8)]

        [SerializeField]
        public Map[] loadingScreens = null;

        [Space(4)]

        [SerializeField]
        [HideIf(nameof(LoadingScreensEmpty))]
        [ValueDropdown(nameof(LoadingScreenDropdownEditor), AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        public Map defaultLoadingScreen = null;

        private bool LoadingScreensEmpty() { return !loadingScreens.Any(); }

        public IList<ValueDropdownItem<Map>> LoadingScreenDropdownEditor() 
        {
            #if UNITY_EDITOR
            return loadingScreens.Select(m => new ValueDropdownItem<Map>(m.GetSceneNameEditor(), m))
                                 .Prepend(new ValueDropdownItem<Map>("None", new Map()))
                                 .ToArray();
            #else
            return null;
            #endif
        }
    }
}