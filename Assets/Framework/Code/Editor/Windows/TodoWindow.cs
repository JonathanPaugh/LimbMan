using System.Linq;
using UnityEditor;
using UnityEngine;
using Jape;
using Sirenix.OdinInspector;

namespace JapeEditor
{
    public class TodoWindow : Window
    {
        protected override string Title => "Todo";

        private const int Columns = 3;

        private const int HeaderHeight = 24;
        private const int HeaderFontSize = (int)(HeaderHeight * 0.75);

        private const int BodyHeight = 128;

        private const int ButtonPadding = 1;

        [SerializeField]
        [HideLabel]
        [EnumToggleButtons]
        private Sector sector = Sector.Game;

        private Field[] fields;

        private Vector2 scrollPosition;

        private Cache<GUIContent> deleteIcon = Cache<GUIContent>.CreateEditorManaged(() =>
        {
            return new GUIContent(Database.GetAsset<Texture2D>("IconDelete").Load<Texture2D>().Colorize(Color.red));
        });

        private Field[] GetFields()
        {
            return DataType.FindAll<Todo>()
                           .Where(t => t.CurrentSector() == sector)
                           .Select(t => new Field(t, deleteIcon.Value))
                           .Concat(new Field[] { new CreateField(sector) })
                           .ToArray();
        }

        protected override void Draw()
        {
            fields = GetFields();

            GUILayout.Space(4);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);

            GUILayout.BeginVertical();

            for (int i = 0; i < fields.Length; i += Columns)
            {
                DrawFieldRow(i);
            }

            EditorGUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            Save();
        }

        private void DrawFieldRow(int index)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = index; i < fields.Length && i < index + Columns; i++)
            {
                fields[i].Draw((position.width / Columns) - 4);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnLostFocus() { Save(); }
        private new void OnDestroy() { Save(); } 

        private void Save()
        {
            if (fields == null) { return; }
            if (EditorGUIUtility.editingTextField) { return; }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Window/Todo", false, -100)]
        private static void Menu() { Open<TodoWindow>(); }

        private class Field
        {
            private readonly Todo todo;
            private readonly GUIContent deleteContent;

            private GUIStyle CloseButtonStyle => new GUIStyle(EditorStyles.miniButton)
            {
                fixedHeight = HeaderHeight + ButtonPadding,
                fixedWidth = HeaderHeight + ButtonPadding,
            }.Offset(new Vector2()).Padding(new RectOffset(ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding));

            private GUIStyle LabelStyle => new GUIStyle(EditorStyles.textField).FontSize(HeaderFontSize);

            private GUIStyle BodyStyle => new(EditorStyles.textField)
            {
                wordWrap = true
            };

            private string Label
            {
                get => Get(todo.label);
                set
                {
                    if (todo.label == value) { return; }
                    todo.label = value;
                    EditorUtility.SetDirty(todo);
                }
            }

            private string Body
            {
                get => Get(todo.text);
                set
                {
                    if (todo.text == value) { return; }
                    todo.text = value;
                    EditorUtility.SetDirty(todo);
                }
            }

            private string Get(string value)
            {
                if (value == null) { return string.Empty; }
                return value;
            }

            public Field(Todo todo, GUIContent delete)
            {
                this.todo = todo;
                deleteContent = delete;
            }

            private void Delete()
            {
                todo.DeleteEditor();
            }

            public virtual void Draw(float width)
            {
                bool delete = false;

                GUILayout.BeginVertical(GUILayout.Width(width));

                GUILayout.BeginHorizontal();

                Label = EditorGUILayout.TextField(Label, LabelStyle, GUILayout.Height(HeaderHeight));

                if (GUILayout.Button(deleteContent, CloseButtonStyle))
                {
                    delete = true;
                }

                GUILayout.EndHorizontal();

                Body = EditorGUILayout.TextArea(Body, BodyStyle, GUILayout.Height(BodyHeight));

                GUILayout.EndVertical();

                if (delete)
                {
                    Delete();
                }
            }
        }

        private class CreateField : Field
        {
            private readonly Sector sector;

            private Cache<GUIContent> addIcon = Cache<GUIContent>.CreateEditorManaged(() =>
            {
                return new GUIContent(Database.GetAsset<Texture2D>("IconPlus").Load<Texture2D>());
            });

            public CreateField(Sector sector) : base(null, null)
            {
                this.sector = sector;
            }

            public override void Draw(float width)
            {
                if (GUILayout.Button(addIcon.Value, GUILayout.Width(width), GUILayout.Height(HeaderHeight + BodyHeight + 1)))
                {
                    switch (sector)
                    {
                        case Sector.Game:
                        {
                            string path = SystemData.GetPath<Todo>(Sector.Game);
                            DataType.CreateData<Todo>(path, IO.Editor.FileIndexName("GameTodo", path));
                            break;
                        }
                        
                        case Sector.Framework:
                        {
                            string path = SystemData.GetPath<Todo>(Sector.Framework);
                            DataType.CreateData<Todo>(path, IO.Editor.FileIndexName("FrameworkTodo", path));
                            break;
                        }   
                    }
                }
            }
        }
    }
}