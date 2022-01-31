using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
	public partial class Properties : Mono
    {
        internal static List<string> runtimeIds = new List<string>();

        public new string Key => $"{GetType().FullName}_{gameObject.Identifier()}";

        [OnInspectorInit(nameof(InitInspector))]

        [PropertySpace(4)]
        [TabGroup("Tabs", "General")]

        [PropertyOrder(-1)]
        [SerializeField]
        [EnableIf(nameof(IsPrefab))]
        [HideLabel]
        internal string alias;
        
        [PropertySpace(4)]
        [HorizontalGroup("Tabs/General/Id")]

        [PropertyOrder(-1)]
        [SerializeField]
        [HideLabel, ReadOnly]
        private string id;
        internal string Id
        {
            get => id;
            set
            {
                if (Game.IsRunning && id != null)
                {
                    if (runtimeIds.Contains(id))
                    {
                        runtimeIds.Remove(id);
                    }
                } 
                id = value;
                if (Game.IsRunning && id != null) { runtimeIds.Add(id); }
            }
        }

        private bool idActive;

        [PropertySpace(4)]
        [HorizontalGroup("Tabs/General/Id", MaxWidth = 42)]

        [PropertyOrder(-1)]
        [ShowInInspector]
        [DisableIf(nameof(IsPrefab))]
        [Button(ButtonHeight = 20)]
        private void Copy() { Id.Copy(); }

        [PropertySpace(8)]
        [TabGroup("Tabs", "General")]

        [SerializeField]
        [HideIf(nameof(NoPlayer))]
        internal int player;

        [TabGroup("Tabs", "General")]

        [SerializeField]
        [HideIf(nameof(NoTeam))]
        [ValueDropdown(nameof(GetTeams))]
        internal Team team;

        [PropertySpace(8)]
        [TabGroup("Tabs", "General")]

        [SerializeField]
        internal List<Tag> tags = new List<Tag>();

        [TabGroup("Tabs", "Save")]

        [SerializeField]
        internal bool save;

        [TabGroup("Tabs", "Save")]

        [SerializeField]
        [ShowIf(nameof(save))]
        internal bool savePosition = true;

        [PropertySpace(4)]
        [TabGroup("Tabs", "Save")]

        [SerializeField]
        [ShowIf(nameof(save))]
        [OnInspectorGUI(nameof(SetSavedElements))]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        [LabelText("Elements")]
        internal List<SavedElement> savedElements = new List<SavedElement>();

        [HideInInspector]
        public Action<GameObject> OnDisabled = delegate {};

        internal Rigidbody Rigidbody => GetComponent<Rigidbody>();
        internal Rigidbody2D Rigidbody2D => GetComponent<Rigidbody2D>();

        internal Collider[] Colliders => GetComponents<Collider>();
        internal Collider2D[] Colliders2D => GetComponents<Collider2D>();

        internal static Properties Create(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Properties properties))
            {
                return properties;
            }

            #if UNITY_EDITOR

            if (UnityEditor.PrefabUtility.IsPartOfImmutablePrefab(gameObject))
            {
                Log.Warning("Cannot add properties to immutable prefab");
                return null;
            }

            #endif

            properties = gameObject.AddComponent<Properties>();

            if (properties == null)
            {
                Log.Warning("Error Creating Properties");
            }

            return properties;
        }

        private void SetSavedElements()
        {
            ValidateSaveElements();
            Element[] elements = GetComponents<Element>().Where(e => e.Saved).ToArray();
            savedElements.RemoveAll(e => e.element == null);
            foreach (Element element in elements.Where(element => savedElements.All(e => e.element != element)))
            {
                savedElements.Add(new SavedElement
                {
                    element = element,
                    save = true
                });
            }
        }

        internal new void ApplyForce(Vector3 force, ForceMode mode, bool useMass = true)
        {
            if (Rigidbody != null)
            {
                switch (mode)
                {
                    case ForceMode.Default:
                        Rigidbody.AddForce(force, useMass ? UnityEngine.ForceMode.Force : UnityEngine.ForceMode.Acceleration);
                        break;

                    case ForceMode.Instant:
                        Rigidbody.AddForce(force, useMass ? UnityEngine.ForceMode.Impulse : UnityEngine.ForceMode.VelocityChange);
                        break;
                }
                return;
            }

            if (Rigidbody2D != null)
            {
                switch (mode)
                {
                    case ForceMode.Default:
                        Rigidbody2D.AddForce(force, ForceMode2D.Force);
                        break;

                    case ForceMode.Instant:
                        Rigidbody2D.AddForce(force, ForceMode2D.Impulse);
                        break;
                }
                return;
            }
        }

        internal bool HasRigidbody()
        {
            return Rigidbody != null || 
                   Rigidbody2D != null;
        }

        internal int ColliderCount()
        {
            return Colliders.Length + Colliders2D.Length;
        }

        private bool NoPlayer() { return player == 0; }
        
        private bool NoTeam() { return team == null; }
        private object GetTeams() { return DataType.Dropdown<Team>(); }

        internal void AddTag(Tag tag) { tags.Add(tag); }
        internal void RemoveTag(Tag tag) { tags.Remove(tag); }
        internal bool HasTag(Tag tag) { return tags.Contains(tag); }

        internal void Send(Element.IReceivable receivable) { foreach (Element element in GetComponents<Element>()) { element.Send(receivable); }}

        private bool CanSave()
        {
            if (string.IsNullOrEmpty(gameObject.Identifier())) { return false; }
            return save;
        }

        internal bool CanSaveElement(Element element)
        {
            if (!save) { return false; }
            if (savedElements.All(e => e.element != element)) { return false; }
            return savedElements.First(e => e.element == element).save;
        }

        internal void SaveAll()
        {
            if (!CanSave()) { Log.Warning($"Unable to SaveAll() on {gameObject.name}"); }
            foreach (SavedElement element in savedElements) { element.element.Save(); }
            Save();
        }

        public void Save()
        {
            if (!CanSave()) { return; }

            Status status = new Status { Key = Key };

            if (gameObject.Properties().savePosition) { status.position = transform.position; }

            Jape.Status.Save(status);
        }

        public void Load()
        {
            if (!CanSave()) { return; }

            Status status = Jape.Status.Load<Status>(Key);

            if (status == null) { return; }

            if (gameObject.Properties().savePosition) { transform.position = status.position; }
        }

        private void ValidateSaveElements()
        {
            foreach (SavedElement element in savedElements)
            {
                SavedElement[] matches = savedElements.Where
                (
                    e => element.save 
                    && e.save 
                    && e.element.GetType() == element.element.GetType()
                ).ToArray();

                if (matches.Length <= 1) { continue; } 

                Log.Warning($"Cannot have multiple saved {element.element.GetType()} elements", "Deactivated saving for both elements");

                foreach (SavedElement match in matches)
                {
                    match.save = false;
                }
            }
        }

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                SaveManager.Instance.OnSaveRequest += Save;
                SaveManager.Instance.OnLoadResponse += Load;
                Load();
            }

            base.Awake(); // Last //
        }

        internal override void OnDestroy()
        {
            base.OnDestroy(); // First //

            if (Game.IsRunning)
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.OnSaveRequest -= Save;
                    SaveManager.Instance.OnLoadResponse -= Load;
                }
            }
        }

        internal override void OnEnable()
        {
            base.OnEnable(); // First //
            if (!Game.IsRunning) { return; }
        }

        internal override void OnDisable()
        {
            base.OnDisable(); // First //
            if (!Game.IsRunning) { return; }
            if (!gameObject.activeSelf)
            {
                OnDisabled?.Invoke(gameObject);
            }
        }

        public void InitInspector()
        {
            #if UNITY_EDITOR
            if (!IsPrefab()) { return; }
            Id = null;
            #endif
        }

        protected override void FrameEditor()
        {
            #if UNITY_EDITOR
            Order();
            #endif
        }

        private void Order()
        {
            #if UNITY_EDITOR
            List<Component> components = gameObject.GetComponents<Component>().ToList();
            int index = components.FindIndex(c => c == this);
            Enumeration.Repeat(index - 1, () => UnityEditorInternal.ComponentUtility.MoveComponentUp(this));
            #endif
        }

        internal bool IsPrefab()
        {
            #if UNITY_EDITOR
            return (UnityEditor.PrefabUtility.GetPrefabAssetType(gameObject) != UnityEditor.PrefabAssetType.NotAPrefab
                    && UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject) == UnityEditor.PrefabInstanceStatus.NotAPrefab)
                   || UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null;
            #endif
            #pragma warning disable 162
            return false;
            #pragma warning restore 162
        }

        public void SetDirty()
        {
            if (Game.IsRunning) { return; }
            #if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Game.ActiveScene());
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        
        
        /// <summary>
        /// Generate and assign a unique id
        /// </summary>
        public void GenerateId() { Id = Ids.Generate(); }

        #if UNITY_EDITOR

        protected override void EnabledEditor()
        {
            if (IsPrefab()) { return; }
            idActive = true;
            Register();
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += SceneSave;
        }


        protected override void DestroyedEditor()
        {
            if (IsPrefab()) { return; }
            idActive = false;
            Unregister();
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaved -= SceneSave;
        }

        protected override void Cloned()
        {
            if (Ids.Has(Id))
            {
                Id = null;
                Register();
            }
        }

        protected override void Validated()
        {
            Refresh();
        }

        private void Register()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                if (Ids.Has(Id))
                {
                    if (Ids.Get(Id).Map.IsSame(Map.GetActive()))
                    {
                        Refresh();
                    } 
                    else
                    {
                        GenerateId();
                        Ids.Add(this);
                        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += SceneOpen;
                    }
                }
                else
                {
                    Ids.Add(this);
                }
            }
            else
            {
                GenerateId();
                Ids.Add(this);
            }
        }

        private void Unregister()
        {
            if (!gameObject.scene.isLoaded) { return; } 
            Ids.Remove(this);
        }

        private void Refresh()
        {
            if (!idActive) { return; }
            if (string.IsNullOrEmpty(Id)) { return; }
            if (!Ids.Has(this)) { return; }
            Ids.Refresh(this);
        }

        private void SceneOpen(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= SceneOpen;
            SetDirty();
        }

        private void SceneSave(Scene scene) { Refresh(); }

        #endif
    }
}