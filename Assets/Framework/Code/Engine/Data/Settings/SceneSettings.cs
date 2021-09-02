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
        [ValueDropdown(nameof(LoadingScreenDropdown), AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        public Map defaultLoadingScreen = null;

        private bool LoadingScreensEmpty() { return !loadingScreens.Any(); }

        public IList<ValueDropdownItem<Map>> LoadingScreenDropdown() 
        {
            #if UNITY_EDITOR
            return loadingScreens.Select(m => new ValueDropdownItem<Map>(m.GetSceneName(), m)).
                                  Prepend(new ValueDropdownItem<Map>("None", new Map())).
                                  ToArray();
            #endif

            #pragma warning disable 162
            return null;
            #pragma warning restore 162
        }
    }
}