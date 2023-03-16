using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Jape;

namespace JapeEditor
{
    public class EntityWindow : Window
    {
        protected override string Title => "Create Entity";

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;

        [NonSerialized] 
        private Entity.Class entityClass;

        [ValueDropdown(nameof(GetEntityTypes))] 
        public Type entity;
        private IEnumerable<Type> GetEntityTypes() { return Entity.Subclass(entityClass); }

        [PropertySpace(8)]

        [EnableIf(nameof(IsSet))]
        [Button(ButtonSizes.Large)]
        private void Create() 
        { 
            switch (entityClass)
            {
                case Entity.Class.Point:
                    EditorGUIUtility.PingObject(Game.CreateGameObject(entity.CleanName().Replace("Ent", string.Empty), entity));
                    break;

                case Entity.Class.World:
                    CreateWorldEntity();
                    break;
            }
        }

        private bool IsSet() { return entity != null; }

        public static bool ValidateWorldEntity() { return Selection.gameObjects.Any(World.IsWorld); }

        public void CreateWorldEntity()
        {
            foreach (GameObject gameObject in Selection.gameObjects.Where(World.IsWorld))
            {
                gameObject.AddComponent(entity);
            }
        }

        public static void Open(Entity.Class entityClass)
        {
            EntityWindow window = Open<EntityWindow>();
            window.entityClass = entityClass;
        }

        [MenuItem("GameObject/Create Entity", false, -8)]
        private static void Menu()
        {
            EntityWindow window = Open<EntityWindow>();
            window.entityClass = Entity.Class.Point;
        }
    }
}