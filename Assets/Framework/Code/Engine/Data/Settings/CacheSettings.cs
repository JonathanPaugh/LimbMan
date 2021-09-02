using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class CacheSettings : SettingsData
    {
        [SerializeField]
        [AssetsOnly]
        public List<GameObject> startup;
    }
}