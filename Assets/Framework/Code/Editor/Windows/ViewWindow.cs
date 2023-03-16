using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.Utilities.Editor;
using Object = UnityEngine.Object;

namespace JapeEditor
{
    public abstract class ViewWindow<TGroup, TTarget> : Window
    {
        private const float SelectorOverflow = 16;

        private const float ButtonHeight = 32;
        private const float MinButtonWidth = 128;

        protected override float Width => 768;
        protected override float MinHeight => 512;

        protected virtual Action<TGroup, string> AddAction => null;
        protected virtual bool AddInput => false;
        private string input = string.Empty;

        private TGroup Group { get; set; }
        protected TTarget Target { get; set; }

        private int groupIndex = -1;
        private int targetIndex = -1;

        protected Vector2 selectorScrollPosition;
        protected Vector2 targetScrollPosition;

        protected Rect selectorLayout;

        private Cache<GUIContent> inspectIcon = Cache<GUIContent>.CreateEditorManaged(() =>
        {
            return new GUIContent
            {
                image = Database.GetAsset<Texture2D>("IconInspect").Load<Texture2D>(),
                text = "Inspect",
            };
        });

        private Cache<GUIContent> addIcon = Cache<GUIContent>.CreateEditorManaged(() =>
        {
            return new GUIContent(Database.GetAsset<Texture2D>("IconPlus").Load<Texture2D>());
        });

        protected abstract IEnumerable<TGroup> GroupSelections();
        protected abstract IEnumerable<TTarget> TargetSelections(TGroup parent);

        protected virtual string GetGroupLabel(TGroup group) { return group.ToString(); }
        protected virtual string GetTargetLabel(TTarget target) { return target.ToString(); }

        protected abstract void DrawTarget();
        protected abstract void OnSelectTarget(TTarget target);

        protected void ResetSelector()
        {
            Target = default;
            Group = default;
            targetIndex = -1;
            groupIndex = -1;
        }

        private TGroup GroupSelector()
        {
            TGroup[] parents = GroupSelections().ToArray();
            for (int i = 0; i < parents.Length; i++)
            {
                bool active = i == groupIndex;
                active = SelectorButton(active, GetGroupLabel(parents[i]));
                if (active) { groupIndex = i; }
            }
            if (groupIndex >= parents.Length) { return default; }
            return groupIndex != -1 ? parents[groupIndex] : default;
        }

        private TTarget TargetSelector(TGroup group)
        {
            if (group == null) { return default; }
            TTarget[] children = TargetSelections(group).ToArray();
            for (int i = 0; i < children.Length; i++)
            {
                bool active = i == targetIndex;
                active = SelectorButton(active, GetTargetLabel(children[i]));
                if (active)
                {
                    targetIndex = i;
                }
            }
            if (targetIndex >= children.Length) { return default; }
            return targetIndex != -1 ? children[targetIndex] : default;
        }

        protected override void Draw()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            DrawSelectors();
            DrawTargetLow();
            GUILayout.EndHorizontal();
        }

        private void DrawSelectors()
        {
            selectorScrollPosition = GUILayout.BeginScrollView(selectorScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.box, GUILayout.Width(selectorLayout.width + SelectorOverflow));

            GUIHelper.BeginLayoutMeasuring();

            GUILayout.BeginHorizontal();

            DrawParentSelector();
            DrawTargetSelector();

            GUILayout.EndHorizontal();

            selectorLayout = GUIHelper.EndLayoutMeasuring();

            GUILayout.EndScrollView();
        }

        private void DrawParentSelector()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(MinButtonWidth));
            EditorGUI.BeginChangeCheck();
            Group = GroupSelector();

            if (EditorGUI.EndChangeCheck()) { targetIndex = -1; }

            GUILayout.EndVertical();
        }

        private void DrawTargetSelector()
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(MinButtonWidth));

            EditorGUI.BeginChangeCheck();
            Target = TargetSelector(Group);
            if (EditorGUI.EndChangeCheck())
            {
                OnSelectTarget(Target);
            }

            AddButton();

            GUILayout.EndVertical();
        }

        private void DrawTargetLow()
        {
            if (Target == null) { return; }
            targetScrollPosition = GUILayout.BeginScrollView(targetScrollPosition);
            DrawTarget();
            GUILayout.EndScrollView();
        }

        protected void DrawInspectButton(Object target)
        {
            if (GUILayout.Button(inspectIcon.Value, GUILayout.Height(ButtonHeight)))
            {
                ProjectWindowUtil.ShowCreatedAsset(target);
            }

            GUILayout.Space(8);
        }

        protected static bool SelectorButton(bool value, string label) { return GUILayout.Toggle(value, label, GUI.skin.button, GUILayout.Height(ButtonHeight)); }

        private void AddButton()
        {
            if (AddAction == null) { return; }
            if (Group == null) { return; }

            if (AddInput)
            {
                input = GUILayout.TextField(input);
            }

            if (GUILayout.Button(addIcon.Value, GUILayout.Height(ButtonHeight)))
            {
                AddAction.Invoke(Group, input);
            }
        }
    }
}