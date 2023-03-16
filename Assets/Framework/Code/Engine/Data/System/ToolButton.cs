using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class ToolButton : SystemData
    {
        protected new static string Path => IO.JoinPath(SystemPath, "ToolButtons");

        private const int DefaultMaxSize = 64;
        private const float BadgeProportion = 0.5f;

        public enum UseAction { Window, Behaviour, Separator };

        [SerializeField] 
        private Probability size = 100;

        [PropertySpace(8)]
        
        [SerializeField, HideLabel, UsedImplicitly]
        private Button button = new Button();

        [PropertySpace(8)]

        [SerializeField]
        [PropertyOrder(1)]
        [LabelText("Modifier")]
        private bool useModifier = false;

        [PropertyOrder(1)]
        [ShowIf(nameof(useModifier))]
        [SerializeField, HideLabel, UsedImplicitly]
        private Button modifier = new Button();

        public Button GetButton()
        {
            if (useModifier && UnityEngine.Event.current.shift) { return modifier; }
            return button;
        }

        public void Draw(float maxSize = -1)
        {
            Button button = GetButton();

            float size = GetSize();
            float badgeSize = GetBadgeSize();

            switch (button.Action)
            {
                case UseAction.Behaviour:
                    if (!button.Enabled()) { GUI.enabled = false; }
                    break;

                case UseAction.Separator:
                    GUILayout.FlexibleSpace();
                    return;
            }

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            button.value = button.Use(GUILayout.Toggle(button.value, button.GetIcon(), GUI.skin.button, GUILayout.Width(size), GUILayout.Height(size)));

            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.Label(new Rect(rect.x + size - badgeSize, rect.y, badgeSize, badgeSize), button.GetBadge());

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUI.enabled = true;

            float GetSize()
            {
                return maxSize >= 0 ? 
                       this.size * maxSize : 
                       this.size * DefaultMaxSize;
            }

            float GetBadgeSize()
            {
                return BadgeProportion * GetSize();
            }
        }


        private static Type WindowClass() { return Member.Class(Assemblies.FrameworkEditor, "Window", WindowSolver); }
        private static Type WindowSolver(Type[] types) { return types.FirstOrDefault(t => t.Name == "Window"); }

        [Serializable]
        public class Button
        {
            public string label;

            public Button()
            {
                iconCache = Cache<GUIContent>.CreateEditorManaged(() =>
                {
                    return icon.IsSet ? new GUIContent(icon.Render()) : GUIContent.none;
                });

                badgeCache = Cache<GUIContent>.CreateEditorManaged(() =>
                {
                    return badge.IsSet ? new GUIContent(badge.Render()) : GUIContent.none;
                });
            }

            [SerializeField, HideInInspector]
            [UsedImplicitly]
            private Icon icon;

            [ShowInInspector]
            private Icon Icon 
            {
                get => icon;
                set
                {
                    icon = value;
                    iconCache.SetDirty();
                }
            }

            private readonly Cache<GUIContent> iconCache;
            public GUIContent GetIcon() => iconCache != null ? iconCache.Value : GUIContent.none;

            [SerializeField, HideInInspector]
            [UsedImplicitly]
            private Icon badge;

            [ShowInInspector]
            [PropertySpace(4)]
            private Icon Badge 
            {
                get => badge;
                set
                {
                    badge = value;
                    badgeCache.SetDirty();
                }
            }

            private readonly Cache<GUIContent> badgeCache;
            public GUIContent GetBadge() => badgeCache != null ? badgeCache.Value : GUIContent.none;

            [SerializeField, HideInInspector]
            public bool value;

            [SerializeField, HideInInspector] 
            private UseAction action;

            [PropertySpace(8)]

            [ShowInInspector]
            public UseAction Action
            {
                get { return action; }
                set
                {
                    if (action != value)
                    {
                        window = null;
                        behaviour = null;
                        behaviourInstance = null;
                    }
                    if (value == UseAction.Behaviour)
                    {
                        behaviour = new Behaviour.Generator<GuiButtonBehaviour>();
                    }
                    action = value;
                }
            }

            [PropertySpace(8)]

            [PropertyOrder(1)]
            [ShowIf(nameof(action), UseAction.Window)]
            [ValueDropdown(nameof(WindowDropdown))]
            [SerializeField]
            public string window;

            private IEnumerable<Type> GetWindowClasses() => WindowClass().GetSubclass(Assemblies.GetJapeEditor());
            public IList<ValueDropdownItem<string>> WindowDropdown() => GetWindowClasses().Select(c => new ValueDropdownItem<string>(c.Name, c.AssemblyQualifiedName)).ToArray();
            
            [PropertySpace(8)]

            [PropertyOrder(1)]
            [SerializeField]
            [HideReferenceObjectPicker]
            [ShowIf(nameof(IsBehaviour))]
            [Eject]
            private Behaviour.Generator<GuiButtonBehaviour> behaviour;

            [SerializeField, HideInInspector] 
            private GuiButtonBehaviour behaviourInstance;

            private GuiButtonBehaviour GetBehaviour()
            {
                if (!IsSet()) { return null; }
                if (behaviourInstance != null) { return behaviourInstance; }
                behaviourInstance = behaviour.CreateInstance();
                return behaviourInstance;
            }

            public bool Use(bool value)
            {
                switch (action)
                {
                    case UseAction.Window:
                        if (!value) { return false; }
                        if (string.IsNullOrEmpty(window)) { return false; }
                        MethodInfo method = WindowClass().GetMethod("Open");
                        MethodInfo generic = method.MakeGenericMethod(GetWindowClasses().First(c => c.AssemblyQualifiedName == window));
                        generic.Invoke(null, null);
                        return false;

                    case UseAction.Behaviour:
                        return behaviourInstance != null && behaviourInstance.Use(value);

                    default: return false;
                }
            }

            public bool Enabled() { return GetBehaviour() != null && GetBehaviour().Enabled(); }

            public bool IsSet() { return behaviour != null; }
            public bool IsSeparator() { return action == UseAction.Separator; }
            public bool IsBehaviour() { return action == UseAction.Behaviour; }
        }
    }
}