using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public class RouterWindow : Window
    {
        private const int SectionLeft = 96;

        private enum Mode { Simple, Smart }

        protected override string Title => entity != null ? $"{entity.name} ({entity.GetType()})" : string.Empty;

        private Entity entity; 
        private Entity.Routing currentRouting;

        private int outputField;
        private int targetField;
        private int actionField;
        private string delayField;

        private Vector2 scroll;

        private Color defaultColor;
        private static Color SelectColor => Color.green;
        private static Color WarningColor => Color.red;

        [SerializeField]
        [HideLabel]
        private Mode mode = Mode.Smart;

        private static GUIStyle LabelStyle => new(GUI.skin.label);
        private static GUIStyle TextStyle => new(GUI.skin.textField);
        private static GUIStyle DropdownStyle => new(GUI.skin.GetStyle("Popup"));
        private static GUIStyle MiniButtonStyle => new(GUI.skin.button.Padding(new RectOffset(1, 1, 1, 1)));

        private Cache<GUIContent> deleteIcon = Cache<GUIContent>.CreateEditorManaged(() =>
        {
            return new GUIContent(Database.GetAsset<Texture2D>("IconDelete").Load<Texture2D>().Colorize(Color.red));
        });

        private float WindowWidth() { return position.width - 8; }
        private float ScrollHeight() { return position.height * 0.6f; }
        private float RoutingWidth() { return WindowWidth() / FieldCount(); }
        private static float RoutingHeight() { return 16; }
        private static int FieldCount() { return Enum.GetNames(typeof(Entity.Routing.Field)).Length; }

        private static bool IsFieldValid(List<string> list, string value) { return string.IsNullOrEmpty(value) || list.Contains(value); }
        private static bool IsFieldDuplicate(List<string> list, string value) { return list.Count(s => s == value) > 1; }

        private bool PrefabMode() { return Prefab.IsPrefab(entity.gameObject); }

        private IEnumerable<Entity> FindSceneEntities(Entity.Routing routing)
        {
            IEnumerable<Entity> entities = Prefab.IsOpen() ?
                                           Prefab.GetActive().prefabContentsRoot.GetComponentsInChildren<Entity>() :
                                           Jape.Game.Find<Entity>();
            
            if (routing.Mode == Entity.Routing.TargetMode.Local)
            {
                Transform parent = entity.transform.parent;
                if (parent != null)
                {
                    return entities.Where(e => parent.gameObject.GetComponentsInChildren<Entity>(true).Contains(e));
                }
            } 

            return entities;
        }

        private IEnumerable<Entity> FindTargets(Entity.Routing routing)
        {
            return PrefabMode() ?
                   Prefab.GetRoot(entity.gameObject).GetComponentsInChildren<Entity>() :
                   FindSceneEntities(routing);
        }

        private IEnumerable<Entity> CurrentTargets(Entity.Routing routing) { return FindTargets(routing).Where(e => e.name == routing.target); }

        private IEnumerable<string> FindActions(Entity.Routing routing)
        {
            List<string> actions = new();
            foreach (Entity entity in CurrentTargets(routing)) { actions.AddRange(entity.TypeActions().Select(m => m.Name)); }
            Enumeration.Repeat(CurrentTargets(routing).Count(), () => actions.AddRange(Entity.BaseActions().Select(m => m.Name)));
            return actions;
        }

        private static GUIStyle ValidateField(List<string> list, string value, GUIStyle style)
        {
            if (!IsFieldValid(list, value)) { list.Add(value); style.FontColor(WarningColor); return style; }
            if (IsFieldDuplicate(list, value)) { style.FontColor(new Color(1, 0.5f, 0)); }
            return style;
        }


        private static GUIStyle ValidateFieldTarget(List<string> list, string value, GUIStyle style)
        {
            if (value == "Self" || value == "Caller") { return style; }
            return ValidateField(list, value, style);
        }

        private void ResetRouting() { currentRouting = Entity.Routing.Create(entity); }
        private void ResetColor() { GUI.color = defaultColor; }

        protected override void Init() { defaultColor = GUI.color; }

        protected override void Draw()
        {
            if (entity == null) { return; }
            if (currentRouting == null) { ResetRouting(); } 

            DrawSelector();

            if (!entity.GetRouter().GetRoutings().Contains(currentRouting)) { GUI.enabled = false; }

            DrawFields();

            GUI.enabled = true;

            DrawButtons();

            GUILayout.FlexibleSpace();

            if (GUI.Button(new Rect(0, 0, WindowWidth(), ScrollHeight()), string.Empty, GUIStyle.none)) { ResetRouting(); }

            if (GUI.Button(new Rect(Vector2.zero, position.size), string.Empty, GUIStyle.none)) { GUI.FocusControl(null); }
        }

        private void DrawSelector()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, 
                                                     false, 
                                                     false, 
                                                     GUIStyle.none, 
                                                     GUIStyle.none, 
                                                     EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box, 
                                                     GUILayout.Width(WindowWidth()), 
                                                     GUILayout.Height(ScrollHeight()));
            
            DrawLabels();

            DrawContent();

            GUILayout.EndScrollView();
        }
        
        private void DrawLabels()
        {
            const int ButtonSize = 16;
            const int ButtonOffset = 4;

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal();
            SortingButton(Entity.Routing.Field.Output);
            GUIHelper.PushColor(Color.black);
            EditorGUILayout.LabelField(nameof(currentRouting.output).SerializationName(), 
                                       LabelStyle.FontStyle(FontStyle.Bold), 
                                       GUILayout.Width((RoutingWidth() * 0.5f) - ButtonSize - ButtonOffset), 
                                       GUILayout.Height(RoutingHeight()));
            GUIHelper.PopColor();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            SortingButton(Entity.Routing.Field.Target);
            GUIHelper.PushColor(Color.black);
            EditorGUILayout.LabelField(nameof(currentRouting.target).SerializationName(), 
                                       LabelStyle.FontStyle(FontStyle.Bold), 
                                       GUILayout.Width(RoutingWidth() - ButtonSize - ButtonOffset), 
                                       GUILayout.Height(RoutingHeight()));
            GUIHelper.PopColor();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            SortingButton(Entity.Routing.Field.Action);
            GUIHelper.PushColor(Color.black);
            EditorGUILayout.LabelField(nameof(currentRouting.action).SerializationName(), 
                                       LabelStyle.FontStyle(FontStyle.Bold), 
                                       GUILayout.Width((RoutingWidth() * 0.5f) - ButtonSize - ButtonOffset), 
                                       GUILayout.Height(RoutingHeight()));
            GUIHelper.PopColor();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            SortingButton(Entity.Routing.Field.Parameters);
            GUIHelper.PushColor(Color.black);
            EditorGUILayout.LabelField(nameof(currentRouting.parameters).SerializationName(), 
                                       LabelStyle.FontStyle(FontStyle.Bold), 
                                       GUILayout.Width((RoutingWidth() * 2) - ButtonSize - ButtonOffset), 
                                       GUILayout.Height(RoutingHeight()));
            GUIHelper.PopColor();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            SortingButton(Entity.Routing.Field.Delay);
            GUIHelper.PushColor(Color.black);
            EditorGUILayout.LabelField(nameof(currentRouting.delay).SerializationName(), 
                                       LabelStyle.FontStyle(FontStyle.Bold), 
                                       GUILayout.Width(RoutingWidth() - ButtonSize - ButtonOffset), 
                                       GUILayout.Height(RoutingHeight()));
            GUIHelper.PopColor();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            void SortingButton(Entity.Routing.Field field)
            {
                if (GUILayout.Button(entity.GetRouter().GetSorting().GetContent(field), 
                                     MiniButtonStyle, 
                                     GUILayout.Width(ButtonSize), 
                                     GUILayout.Height(RoutingHeight())))
                {
                    entity.GetRouter().GetSorting().ToggleSorting(field, entity.GetRouter());
                }
            }
        }

        private void DrawContent()
        {
            List<Entity.Routing> deleted = new();

            int i = 0;
            foreach (Entity.Routing routing in entity.GetRouter().GetRoutings())
            {
                if (routing == null) { deleted.Add(routing); continue; }
                if (DrawSelection(i)) { deleted.Add(routing); }
                DrawRouting(routing);
                i++;
            }

            foreach (Entity.Routing routing in deleted) { entity.GetRouter().Remove(routing); }
        }

        private bool DrawSelection(int index)
        {
            const int Space = 21;

            Entity.Routing routing = entity.GetRouter().GetRoutings()[index];

            Rect selectionPosition = new(0, Space + (index * (RoutingHeight() + 2)), WindowWidth(), RoutingHeight() + 1);
            Rect deletePosition = new(position.width - 26, Space + (index * (RoutingHeight() + 2)), 16, RoutingHeight());
            
            if (GUI.Button(deletePosition, GUIContent.none)) { return true; }
            GUI.Label(deletePosition, deleteIcon.Value, GUIStyle.none);

            GUI.color = index.IsEven() ? new Color(0, 0, 0, 0.1f) : new Color(0, 0, 0, 0.2f);

            if (currentRouting == routing) { GUI.color = new Color(SelectColor.r, SelectColor.g, SelectColor.b, GUI.color.a); }

            if (GUI.Button(selectionPosition, GUIContent.none, EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box))
            {
                GUI.FocusControl(null); 
                currentRouting = routing;
            }

            ResetColor();

            return false;
        }

        private void DrawRouting(Entity.Routing routing)
        {
            GUILayout.BeginHorizontal();

            string name = routing.output ?? string.Empty;
            EditorGUILayout.LabelField(name, ValidateField(entity.GetOutputs().ToList(), routing.output, LabelStyle.FontColor(Color.black)), GUILayout.Width(RoutingWidth() * 0.5f), GUILayout.Height(RoutingHeight()));

            name = routing.target ?? string.Empty;
            string target = routing.Mode == Entity.Routing.TargetMode.Local ?
                            name.Insert(0, $"{routing.LocalTarget().name} >> ") :
                            name;

            EditorGUILayout.LabelField(target, ValidateFieldTarget(FindSceneEntities(routing).Select(e => e.gameObject.name).ToList(), routing.target, LabelStyle.FontColor(Color.black)), GUILayout.Width(RoutingWidth()), GUILayout.Height(RoutingHeight()));

            name = routing.action ?? string.Empty;
            EditorGUILayout.LabelField(name, ValidateField(FindActions(routing).ToList(), routing.action, LabelStyle.FontColor(Color.black)), GUILayout.Width(RoutingWidth() * 0.5f), GUILayout.Height(RoutingHeight()));

            name = routing.parameters == null ? string.Empty : string.Join(", ", routing.parameters);
            EditorGUILayout.LabelField(name, LabelStyle.FontColor(Color.black), GUILayout.Width(RoutingWidth() * 2), GUILayout.Height(RoutingHeight()));

            name = routing.delay.ToString(CultureInfo.InvariantCulture);
            EditorGUILayout.LabelField(name, LabelStyle.FontColor(Color.black), GUILayout.Width(RoutingWidth()), GUILayout.Height(RoutingHeight()));

            GUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create", GUILayout.Height(32), GUILayout.Width(position.width * 0.4f)))
            {
                GUI.FocusControl(null);
                currentRouting = Entity.Routing.Create(entity);
                entity.GetRouter().Add(currentRouting);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Duplicate", GUILayout.Height(32), GUILayout.Width(position.width * 0.4f)))
            {
                GUI.FocusControl(null);
                entity.GetRouter().Add(currentRouting.Clone());
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        private void DrawFields()
        {
            GUIStyle style;

            EditorGUI.BeginChangeCheck();

            GUILayout.FlexibleSpace();
            DrawOutputs();
            GUILayout.FlexibleSpace();
            DrawSends();
            GUILayout.FlexibleSpace();
            DrawTargets();
            GUILayout.FlexibleSpace();
            DrawActions();
            GUILayout.FlexibleSpace();
            DrawParameters();
            GUILayout.FlexibleSpace();
            DrawDelay();
            GUILayout.FlexibleSpace();

            if (EditorGUI.EndChangeCheck())
            {
                entity.GetRouter().GetSorting().Sort(entity.GetRouter());
            }

            void DrawOutputs()
            {
                List<string> outputs = entity.GetOutputs().ToList();

                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(nameof(currentRouting.output).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));

                switch (mode)
                {
                    case Mode.Simple: Simple(); break;
                    case Mode.Smart: Smart(); break;
                }

                GUILayout.EndHorizontal();

                void Simple()
                {
                    style = ValidateField(outputs, currentRouting.output, TextStyle);
                    currentRouting.output = EditorGUILayout.TextField(currentRouting.output, style);
                } 

                void Smart()
                { 
                    style = ValidateField(outputs, currentRouting.output, DropdownStyle);
                    outputField = EditorGUILayout.Popup(outputs.IndexOf(currentRouting.output), outputs.ToArray(), style);
                    if (outputField >= 0) { currentRouting.output = outputs[outputField]; }
                }
            }

            void DrawSends()
            {
                if (string.IsNullOrEmpty(currentRouting.output)) { return; }
                if (entity.Sends().Any(s => s.output.ToString() == currentRouting.output))
                {
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(nameof(Entity.Sends).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));

                    List<Entity.Send> sends = entity.Sends().Where(s => s.output.ToString() == currentRouting.output).ToList();

                    foreach (Entity.Send send in sends)
                    {
                        GUILayout.BeginVertical(new GUIStyle(GUI.skin.box));
                        GUILayout.Label($"{nameof(Entity.Send)} {sends.IndexOf(send) + 1}");
                        GUILayout.Label($"{send.name} - {send.type}".RemoveNamespace());
                        GUILayout.EndVertical();
                    }

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();
                }
            }

            void DrawTargets()
            {
                List<string> targets = FindTargets(currentRouting).Select(e => e.name).ToList();
                List<string> validTargets = FindSceneEntities(currentRouting).Select(e => e.gameObject.name).ToList();

                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(nameof(currentRouting.target).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));

                currentRouting.Mode = (Entity.Routing.TargetMode)EditorGUILayout.EnumPopup(currentRouting.Mode, GUILayout.Width(64));

                if (currentRouting.Mode != Entity.Routing.TargetMode.Self)
                {
                    switch (mode)
                    {
                        case Mode.Simple: Simple(); break;
                        case Mode.Smart: Smart(); break;
                    }
                }

                GUILayout.EndHorizontal();

                void Simple()
                {
                    style = ValidateFieldTarget(validTargets, currentRouting.target, TextStyle);
                    currentRouting.target = EditorGUILayout.TextField(currentRouting.target, style);
                }

                void Smart()
                {
                    style = ValidateFieldTarget(validTargets, currentRouting.target, DropdownStyle);
                    targetField = EditorGUILayout.Popup(targets.ToList().IndexOf(currentRouting.target), targets.ToArray(), style);
                    if (targetField >= 0 && targetField < targets.Count) { currentRouting.target = targets[targetField]; }
                }
            }

            void DrawActions()
            {
                List<string> actions = FindActions(currentRouting).ToList();

                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(nameof(currentRouting.action).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));

                if (currentRouting.Mode == Entity.Routing.TargetMode.Self) { Simple(); } 
                else
                {
                    switch (mode)
                    {
                        case Mode.Simple: Simple(); break;
                        case Mode.Smart: Smart(); break;
                    }
                }

                GUILayout.EndHorizontal();

                void Simple()
                {
                    style = ValidateField(actions, currentRouting.action, TextStyle);
                    currentRouting.action = EditorGUILayout.TextField(currentRouting.action, style);
                } 

                void Smart()
                {
                    style = ValidateField(actions, currentRouting.action, DropdownStyle);
                    actionField = EditorGUILayout.Popup(actions.IndexOf(currentRouting.action), actions.ToArray(), style);
                    if (actionField >= 0) { currentRouting.action = actions[actionField]; }
                }
            }

            void DrawParameters()
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(nameof(currentRouting.parameters).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));

                if (currentRouting.Mode == Entity.Routing.TargetMode.Self) { Simple(); } 
                else
                {
                    switch (mode)
                    {
                        case Mode.Simple: Simple(); break;
                        case Mode.Smart: Smart(); break;
                    }
                }

                GUILayout.EndHorizontal();

                void Simple()
                {
                    string parameters = ParametersUnset() ? 
                                        string.Empty :
                                        string.Join(", ", currentRouting.parameters);

                    currentRouting.parameters = EditorGUILayout.TextField(parameters).Split(',').Select(s => s.Trim()).ToArray();
                } 

                void Smart()
                {
                    GUILayout.BeginVertical();

                    Dictionary<string, string> signatures = new();
                    if (string.IsNullOrEmpty(currentRouting.target) || !IsFieldValid(FindActions(currentRouting).ToList(), currentRouting.action))
                    {
                        if (!SignaturesUnset())
                        {
                            signatures = currentRouting.signatureNames.ToDictionary
                            (
                                n => n,
                                n => currentRouting.
                                     signatureTypes[currentRouting.
                                                    signatureNames.
                                                    ToList().
                                                    IndexOf(n)]
                            );
                        }
                    }
                    else
                    {
                        signatures = currentRouting.GetSignatures(currentRouting.GetMethods(CurrentTargets(currentRouting)).FirstOrDefault());
                        currentRouting.SetSignatures(signatures);
                    }
                    
                    if (ParametersUnset()) { currentRouting.parameters = new string[signatures.Count]; }
                    if (currentRouting.parameters.Length < signatures.Count) { Array.Resize(ref currentRouting.parameters, signatures.Count); }

                    List<string> parameterInputs = new();
                    for (int i = 0; i < signatures.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        string name = signatures.Keys.ToArray()[i].SerializationName();
                        string type = signatures.Values.ToArray()[i];

                        GUILayout.Label($"{name} - {type}".RemoveNamespace(), GUILayout.ExpandWidth(false));

                        string input = i < currentRouting.parameters.Length ? 
                                       currentRouting.parameters[i] : 
                                       string.Empty;

                        parameterInputs.Add(EditorGUILayout.TextField(input));

                        GUILayout.EndHorizontal();
                    }

                    for (int i = 0; i < parameterInputs.Count && i < currentRouting.parameters.Length; i++)
                    {
                        currentRouting.parameters[i] = parameterInputs[i];
                    }

                    if (currentRouting.parameters.Length > parameterInputs.Count)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Overflow", GUILayout.ExpandWidth(false));

                        if (GUILayout.Button(deleteIcon.Value, 
                                             MiniButtonStyle, 
                                             GUILayout.Width(18), 
                                             GUILayout.Height(18)))
                        {
                            currentRouting.parameters = currentRouting.parameters.Take(parameterInputs.Count).ToArray();
                            return;
                        }

                        GUILayout.Space(2);

                        int count = currentRouting.parameters.Length - parameterInputs.Count;

                        GUI.enabled = false;

                        string[] overflow = EditorGUILayout.TextField(string.Join(", ", currentRouting.parameters.Reverse().Take(count).Reverse())).Split(',').Select(s => s.Trim()).ToArray();

                        GUI.enabled = true;

                        GUILayout.EndHorizontal();

                        for (int i = 0; i < overflow.Length; i++) { currentRouting.parameters[i + parameterInputs.Count] = overflow[i]; }
                    }

                    GUILayout.EndVertical();
                }

                bool SignaturesUnset()
                {
                    if (currentRouting.signatureNames == null) { return true; }
                    return currentRouting.signatureNames.Length == 0;
                }

                bool ParametersUnset()
                {
                    if (currentRouting.parameters == null) { return true; }
                    return currentRouting.parameters.Length == 0;
                }
            }

            void DrawDelay()
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(nameof(currentRouting.delay).SerializationName(), new GUIStyle(GUI.skin.label).Alignment(TextAnchor.UpperCenter), GUILayout.Width(SectionLeft));
                if (string.IsNullOrEmpty(delayField) || delayField.Last() != '.') { delayField = currentRouting.delay.ToString(CultureInfo.InvariantCulture); }
                delayField = EditorGUILayout.TextField(delayField);
                float.TryParse(delayField, out currentRouting.delay);

                GUILayout.EndHorizontal();
            }
        }

        public static void Call(Entity entity)
        {
            foreach (RouterWindow window in Jape.Game.FindDeep<EditorWindow>().
                                            Where(w => w is RouterWindow).
                                            Cast<RouterWindow>().
                                            Where(w => w.entity == entity))
            {
                window.Show(); return;
            }

            RouterWindow createdWindow = CreateWindow<RouterWindow>();
            createdWindow.entity = entity;
            createdWindow.UpdateTitle();
        }

        public static void CloseAll()
        {
            foreach (EditorWindow window in Jape.Game.FindDeep<EditorWindow>().
                                            Where(w => w is RouterWindow))
            {
                window.Close();
            }
        }
    }
}