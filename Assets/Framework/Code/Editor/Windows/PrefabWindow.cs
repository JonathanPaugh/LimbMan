using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Jape;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace JapeEditor
{
    public class PrefabWindow : Window
    {
        protected override string Title => "Create Prefab";

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;

        [ValueDropdown(nameof(GetPrefabs))]
        public GameObject prefab;

        protected IList<ValueDropdownItem<GameObject>> GetPrefabs() { return Database.GetAssets<GameObject>().Select(r => new ValueDropdownItem<GameObject>(r.AssetPath , r.Load<GameObject>())).ToList(); }

        [PropertySpace(8)]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        private void CreateInstance() { Game.ClonePrefab(prefab); }

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Medium)]
        private void Replicate() { Game.CloneGameObject(prefab); }

        private bool IsSet() { return prefab != null; }

        [MenuItem("GameObject/Create Prefab", false, -7)]
        private static void Menu() { Open<PrefabWindow>(); }
    }
}