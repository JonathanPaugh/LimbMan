using System.Text.RegularExpressions;
using UnityEngine;

namespace Jape
{
	public static class GameObjectExt
    {
        internal static Properties Properties(this GameObject gameObject)
        {
            
            if (gameObject == null) { Log.Write("GameObject is null"); return null; }

            if (gameObject.TryGetComponent(out Properties properties))
            {
                return properties;
            }

            return Jape.Properties.Create(gameObject);
        }

        public static bool HasId(this GameObject gameObject) => !string.IsNullOrEmpty(gameObject.Properties().Id);
        public static bool InScene(this GameObject gameObject) { return gameObject.scene.rootCount != 0; }

        public static void Send(this GameObject gameObject, Element.IReceivable receivable) { gameObject.Properties().Send(receivable); }
        public static void Save(this GameObject gameObject) { gameObject.Properties().SaveAll(); }

        public static string Identifier(this GameObject gameObject) => HasId(gameObject) ? gameObject.Properties().Id : gameObject.Properties().alias;

        public static void GenerateId(this GameObject gameObject)
        {
            gameObject.Properties().GenerateId();
            gameObject.Properties().SetDirtyEditor();
        }

        public static int Player(this GameObject gameObject) { return gameObject.Properties().player; }
        public static void SetPlayer(this GameObject gameObject, int player)
        {
            gameObject.Properties().player = player;
            gameObject.Properties().SetDirtyEditor();
        }

        public static Team[] Teams(this GameObject gameObject) { return gameObject.Properties().Teams; }

        public static bool HasTag(this GameObject gameObject, Tag tag) { return gameObject.Properties().HasTag(tag); }
        public static void AddTag(this GameObject gameObject, Tag tag)
        {
            gameObject.Properties().tags.Add(tag);
            gameObject.Properties().SetDirtyEditor();
        }
        public static void RemoveTag(this GameObject gameObject, Tag tag)
        {
            gameObject.Properties().tags.Remove(tag);
            gameObject.Properties().SetDirtyEditor();
        }

        public static bool HasRigidbody(this GameObject gameObject) { return gameObject.Properties().HasRigidbody(); }

        public static Rigidbody Rigidbody(this GameObject gameObject) { return gameObject.Properties().Rigidbody; }
        public static Rigidbody2D Rigidbody2D(this GameObject gameObject) { return gameObject.Properties().Rigidbody2D; }

        public static int ColliderCount(this GameObject gameObject) { return gameObject.Properties().ColliderCount(); }

        public static Collider[] Colliders(this GameObject gameObject) { return gameObject.Properties().Colliders; }
        public static Collider2D[] Collider2D(this GameObject gameObject) { return gameObject.Properties().Colliders2D; }

        public static void ApplyForce(this GameObject gameObject, Vector3 force, ForceMode mode, bool useMass = true)
        {
            gameObject.Properties().ApplyForce(force, mode, useMass);
        }

        public static bool HasComponent(this GameObject gameObject, System.Type type, bool includeChildren)
        {
            if (includeChildren) { if (gameObject.GetComponentsInChildren(type).Length > 0) { return true; }}
            else { if (gameObject.GetComponents(type).Length > 0) { return true; }}
            return false;
        }

        public static bool HasComponent<T>(this GameObject gameObject, bool includeChildren) where T : Component
        {
            if (includeChildren) { if (gameObject.GetComponentsInChildren<T>().Length > 0) { return true; }}
            else { if (gameObject.GetComponents<T>().Length > 0) { return true; }}
            return false;
        }

        public static void CloneName(this GameObject gameObject) { gameObject.name = gameObject.name.Replace("(Clone)", "").Trim(); }

        public static string IndexName(this GameObject gameObject)
        {
            Match match = Regex.Match(gameObject.name, @"\d+$");
            return match.Success ? $"{gameObject.name.Replace(match.Value, string.Empty)}{int.Parse(match.Value) + 1}" : $"{gameObject.name}{1}";
        }
    }
}