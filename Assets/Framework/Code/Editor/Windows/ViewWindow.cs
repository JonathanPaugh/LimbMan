using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public abstract class ViewWindow : Window
    {
        private const float SelectorOverflow = 8;

        private const float ButtonHeight = 32;
        private const float MinButtonWidth = 128;

        protected override float Width => 768;
        protected override float MinHeight => 512;

        protected virtual bool AddInput => false;
        protected virtual Action<object> AddAction => null;
        
        protected string Input { get; private set; } = string.Empty;

        protected object Parent { get; private set; }
        protected object Child { get; private set; }

        private int parentIndex = -1;
        private int childIndex = -1;

        protected Rect selectorLayout;

        private PropertyTree tree;

        protected abstract IEnumerable<object> ParentSelections();
        protected abstract IEnumerable<object> ChildSelections(object parent);

        protected virtual string GetParentLabel(object parent) { return parent.ToString(); }
        protected virtual string GetChildLabel(object child) { return child.ToString(); }

        protected bool IsSet() { return Child != null; }

        protected void ResetSelector()
        {
            Child = null;
            Parent = null;
            childIndex = -1;
            parentIndex = -1;
        }

        private object ParentSelector()
        {
            object[] parents = ParentSelections().ToArray();
            for (int i = 0; i < parents.Length; i++)
            {
                bool active = i == parentIndex;
                active = SelectorButton(active, GetParentLabel(parents[i]));
                if (active) { parentIndex = i; }
            }
            if (parentIndex >= parents.Length) { return null; }
            return parentIndex != -1 ? parents[parentIndex] : null;
        }

        private object ChildSelector(object parent)
        {
            if (parent == null) { return null; }
            object[] children = ChildSelections(parent).ToArray();
            for (int i = 0; i < children.Length; i++)
            {
                bool active = i == childIndex;
                active = SelectorButton(active, GetChildLabel(children[i]));
                if (active)
                {
                    childIndex = i;
                }
            }
            if (childIndex >= children.Length) { return null; }
            return childIndex != -1 ? children[childIndex] : null;
        }

        protected override void Draw()
        {
            GUILayout.BeginHorizontal();
            DrawSelectors();
            DrawTree();
            GUILayout.EndHorizontal();
        }

        protected virtual void DrawSelectors()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUIHelper.BeginLayoutMeasuring();

            DrawParentSelector();
            DrawChildSelector();

            Rect layout = GUIHelper.EndLayoutMeasuring();
            selectorLayout = layout;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.Box(new Rect(Vector2.zero, new Vector2(layout.width + SelectorOverflow, Mathf.Clamp(layout.height, position.height, Mathf.Infinity) + SelectorOverflow)), GUIContent.none);
        }

        protected virtual void DrawParentSelector()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(MinButtonWidth));
            EditorGUI.BeginChangeCheck();
            Parent = ParentSelector();
            if (EditorGUI.EndChangeCheck()) { childIndex = -1; }
            GUILayout.EndVertical();
        }

        protected virtual void DrawChildSelector()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(MinButtonWidth));

            EditorGUI.BeginChangeCheck();
            Child = ChildSelector(Parent);
            if (EditorGUI.EndChangeCheck()) { tree = PropertyTree.Create(Child); }

            AddButton();

            GUILayout.EndVertical();
        }

        protected virtual void DrawTree()
        {
            if (Child == null) { return; }
            GUILayout.BeginVertical();
            tree?.Draw();
            GUILayout.EndVertical();
        }

        private static bool SelectorButton(bool value, string label) { return GUILayout.Toggle(value, label, GUI.skin.button, GUILayout.Height(ButtonHeight)); }

        private void AddButton()
        {
            if (AddAction == null) { return; }
            if (Parent == null) { return; }
            if (AddInput) { Input = GUILayout.TextField(Input); }
            if (GUILayout.Button(Database.GetAsset("IconMath")?.Load<Texture>(), GUILayout.Height(ButtonHeight)))
            {
                AddAction.Invoke(Parent);
                Input = string.Empty;
            }
        }
    }
}