using System.Text.RegularExpressions;
using UnityEngine;

namespace Jape
{
	public static class GameObjectExt
    {
        internal static Properties Properties(this GameObject gameObject)
        {
            if (gameObject == null) { Log.Write("GameObject is null"); return null; }
            return gameObject.TryGetComponent(out Properties properties) ? properties : gameObject.AddComponent<Properties>();
        }

        public static bool InScene(this GameObject gameObject) { return gameObject.scene.rootCount != 0; }

        public static void Send(this GameObject gameObject, Element.IReceivable receivable) { gameObject.Properties().Send(receivable); }

        public static void Save(this GameObject gameObject) { gameObject.Properties().SaveAll(); }

        public static string Id(this GameObject gameObject) { return gameObject.Properties().Id; }
        public static string Alias(this GameObject gameObject) { return gameObject.Properties().alias; }

        public static void GenerateId(this GameObject gameObject)
        {
            gameObject.Properties().GenerateId();
            gameObject.Properties().SetDirty();
        }

        public static int Player(this GameObject gameObject) { return gameObject.Properties().player; }
        public static void SetPlayer(this GameObject gameObject, int player)
        {
            gameObject.Properties().player = player;
            gameObject.Properties().SetDirty();
        }

        public static Team Team(this GameObject gameObject) { return gameObject.Properties().team; }
        public static void SetTeam(this GameObject gameObject, Team team)
        {
            gameObject.Properties().team = team;
            gameObject.Properties().SetDirty();
        }

        public static bool HasTag(this GameObject gameObject, Tag tag) { return gameObject.Properties().HasTag(tag); }
        public static void AddTag(this GameObject gameObject, Tag tag)
        {
            gameObject.Properties().tags.Add(tag);
            gameObject.Properties().SetDirty();
        }
        public static void RemoveTag(this GameObject gameObject, Tag tag)
        {
            gameObject.Properties().tags.Remove(tag);
            gameObject.Properties().SetDirty();
        }

        public static bool HasRigidbody(this GameObject gameObject) { return gameObject.Properties().HasRigidbody(); }

        public static Rigidbody Rigidbody(this GameObject gameObject) { return gameObject.Properties().Rigidbody; }
        public static Rigidbody2D Rigidbody2D(this GameObject gameObject) { return gameObject.Properties().Rigidbody2D; }

        public static bool HasCollider(this GameObject gameObject) { return gameObject.Properties().HasCollider(); }

        public static Collider Collider(this GameObject gameObject) { return gameObject.Properties().Collider; }
        public static Collider2D Collider2D(this GameObject gameObject) { return gameObject.Properties().Collider2D; }

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

        public static Vector3 Position(this GameObject gameObject) { return gameObject.transform.position; }
        public static Vector3 PositionLocal(this GameObject gameObject) { return gameObject.transform.localPosition; }
        public static Quaternion Rotation(this GameObject gameObject) { return gameObject.transform.rotation; }
        public static Quaternion RotationLocal(this GameObject gameObject) { return gameObject.transform.localRotation; }
        public static Vector3 Scale(this GameObject gameObject) { return gameObject.transform.lossyScale; }
        public static Vector3 ScaleLocal(this GameObject gameObject) { return gameObject.transform.localScale; }
        public static Vector3 Velocity(this GameObject gameObject) { return gameObject.Properties().Velocity(); }

        public static void SetPosition(this GameObject gameObject, Vector3 position) { gameObject.Properties().SetPosition(position); }
        public static void SetPositionLocal(this GameObject gameObject, Vector3 position) { gameObject.Properties().SetPositionLocal(position); }
        public static void SetRotation(this GameObject gameObject, Quaternion rotation) { gameObject.Properties().SetRotation(rotation); }
        public static void SetRotationLocal(this GameObject gameObject, Quaternion rotation) { gameObject.Properties().SetRotationLocal(rotation); }
        public static void SetScaleLocal(this GameObject gameObject, Vector3 scale) { gameObject.Properties().SetScaleLocal(scale); }
        public static void SetVelocity(this GameObject gameObject, Vector3 velocity) { gameObject.Properties().SetVelocity(velocity); }

        public static void Move(this GameObject gameObject, Vector3 offset) { gameObject.Properties().Move(offset); }
        public static void MoveLocal(this GameObject gameObject, Vector3 offset) { gameObject.Properties().MoveLocal(offset); }

        public static void Rotate(this GameObject gameObject, Quaternion offset) { gameObject.Properties().Rotate(offset); }
        public static void RotateLocal(this GameObject gameObject, Quaternion offset) { gameObject.Properties().RotateLocal(offset); }

        public static void ScaleLocal(this GameObject gameObject, Vector3 scale) { gameObject.Properties().ScaleLocal(scale); }

        public static void ApplyForce(this GameObject gameObject, Vector3 force, ForceMode mode, bool useMass = true) { gameObject.Properties().ApplyForce(force, mode, useMass); }

        public static void CloneName(this GameObject gameObject) { gameObject.name = gameObject.name.Replace("(Clone)", "").Trim(); }

        public static string IndexName(this GameObject gameObject)
        {
            Match match = Regex.Match(gameObject.name, @"\d+$");
            return match.Success ? $"{gameObject.name.Replace(match.Value, string.Empty)}{int.Parse(match.Value) + 1}" : $"{gameObject.name}{1}";
        }
    }
}