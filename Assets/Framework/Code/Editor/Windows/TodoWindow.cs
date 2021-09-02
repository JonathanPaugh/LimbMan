using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Jape;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace JapeEditor
{
    public class TodoWindow : Window
    {
        protected override string Title => "Todo";

        [ShowInInspector]
        [HideLabel]
        [EnumToggleButtons]
        private Sector sector = Sector.Game;

        private Todo[] fields;
        private string[] storedFields;

        private void GetFields()
        {
            switch (sector)
            {
                case Sector.Game:
                    fields = DataType.FindAll<Todo>().Where(t => t.CurrentSector() == Sector.Game).ToArray();
                    
                    break;
                case Sector.Framework:
                    fields = DataType.FindAll<Todo>().Where(t => t.CurrentSector() == Sector.Framework).ToArray();
                    break;
            }
            storedFields = new string[fields.Length];
        }

        protected override void Draw()
        {
            GetFields();

            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", GUILayout.Width(position.width - 22), GUILayout.Height(32)))
            {
                switch (sector)
                {
                    case Sector.Game:
                    {
                        string path = SystemData.GetPath<Todo>(Sector.Game);
                        DataType.CreateData<Todo>(path, Directory.FileIndexName("GameTodo", path));
                        break;
                    }
                        
                    case Sector.Framework:
                    {
                        string path = SystemData.GetPath<Todo>(Sector.Framework);
                        DataType.CreateData<Todo>(path, Directory.FileIndexName("FrameworkTodo", path));
                        break;
                    }   
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < fields.Length; i++)
            {
                if (i.IsDivisible(3))
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();

                EditorGUILayout.TextArea(string.Empty, GUILayout.Width((position.width / 3) - 36));

                if (GUILayout.Button(Database.GetAsset("IconDelete").Load<Texture>(),
                                     EditorStyles.miniButton.Offset(new Vector2()).Padding(new RectOffset(1, 1, 1, 1)),
                                     GUILayout.MaxWidth(18),
                                     GUILayout.MaxHeight(18)))
                {
                    fields[i].DestroyAsset();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return;
                }

                GUILayout.EndHorizontal();

                GUIStyle style = new GUIStyle(EditorStyles.textField)
                {
                    wordWrap = true
                };

                if (fields[i].text == null) { fields[i].text = string.Empty; }
                storedFields[i] = EditorGUILayout.TextArea(fields[i].text, style, GUILayout.MinHeight(128), GUILayout.Width((position.width / 3) - 17));

                GUILayout.EndVertical();

                if (IsMiddleField()) { if (i == fields.Length - 2) { GUILayout.Space(12); } }
            }

            if (IsMiddleField()) { GUILayout.FlexibleSpace(); }

            EditorGUILayout.EndHorizontal();

            Save();

            bool IsMiddleField() { return (fields.Length + 1).IsDivisible(3); }
        }

        private bool Initialized() { return fields != null && storedFields != null; }

        private void OnLostFocus() { Save(); }
        private new void OnDestroy() { Save(); } 

        private void Save()
        {
            if (!Initialized()) { return; }

            bool dirty = false;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].text != storedFields[i])
                {
                    fields[i].text = storedFields[i];
                    EditorUtility.SetDirty(fields[i]);
                    dirty = true;
                }
            }

            if (dirty)
            {
                if (EditorGUIUtility.editingTextField) { return; }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Window/Todo", false, -100)]
        private static void Menu() { Open<TodoWindow>(); }
    }
}