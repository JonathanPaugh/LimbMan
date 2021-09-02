using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    public class ToolButton : SystemData
    {
        protected new static string Path => "System/Resources/ToolButtons";

        private const int MaxSize = 64;
        private const int BadgeSize = 24;

        public enum UseAction { Window, Behaviour, Separator };

        [SerializeField] 
        private Probability size = 100;

        [PropertySpace(8)]
        
        [NonSerialized, OdinSerialize]
        [HideLabel, HideReferenceObjectPicker]
        public Button button;

        [PropertySpace(8)]

        [SerializeField]
        [PropertyOrder(1)]
        [LabelText("Modifier")]
        private bool useModifier = false;

        [NonSerialized, OdinSerialize]
        [PropertyOrder(1)]
        [ShowIf(nameof(useModifier))]
        [HideLabel, HideReferenceObjectPicker]
        public Button modifier;

        [PropertySpace(16)]

        [OdinSerialize, ShowInInspector]
        [PropertyOrder(3)]
        [LabelText("Order")]
        [ListDrawerSettings(ShowPaging = false)]
        public List<ToolButton> OrderList
        {
            get { return orderList; }
            set { orderList = value; }
        }

        private static List<ToolButton> orderList;

        public static List<ToolButton> GetOrder() { return orderList ?? (orderList = FindAll<ToolButton>().FirstOrDefault().OrderList); }

        public int GetSize() { return (int)(size * MaxSize); }

        public Button GetButton()
        {
            if (useModifier && UnityEngine.Event.current.shift) { return modifier; }
            return button;
        }

        public void Draw()
        {
            Button button = GetButton();
            GUIContent content = button.Content;

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

            button.value = button.Use(GUILayout.Toggle(button.value, content, GUI.skin.button, GUILayout.Width(GetSize()), GUILayout.Height(GetSize())));

            if (button.badge != null)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.Label(new Rect((rect.x + GetSize()) - BadgeSize, rect.y, BadgeSize, BadgeSize), button.badge);
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private static Type WindowClass() { return Member.Class(Assemblies.FrameworkEditor, "Window", WindowSolver); }
        private static Type WindowSolver(Type[] types) { return types.FirstOrDefault(t => t.Name == "Window"); }

        [Serializable]
        public class Button
        {
            public string label;
            public Texture icon;
            public Texture badge;

            private GUIContent content;
            public GUIContent Content
            {
                get
                {
                    if (content != null) { return content; }
                    content = new GUIContent(icon, label);
                    return content;
                }
            }

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
                    if (value != UseAction.Window) { window = null; }
                    if (value != UseAction.Behaviour) { behaviour = null; }
                    action = value;
                }
            }

            [PropertySpace(8)]

            [PropertyOrder(1)]
            [NonSerialized, OdinSerialize]
            [ShowIf(nameof(action), UseAction.Window)]
            [ValueDropdown(nameof(GetWindowTypes))]
            public Type window;
            private IEnumerable<Type> GetWindowTypes() { return WindowClass().GetSubclass(Assemblies.GetJapeEditor()); } 

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
                        if (window == null) { return false; }
                        MethodInfo method = WindowClass().GetMethod("Open");
                        MethodInfo generic = method.MakeGenericMethod(window);
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