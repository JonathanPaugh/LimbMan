using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.U2D;

namespace Jape
{
	public abstract class World : Element
    {
        private static WorldSettings settings;
        public static WorldSettings Settings => settings != null ? settings : settings = Framework.Settings<WorldSettings>();

        [SerializeField]
        private bool hide;

        [SerializeField, ReadOnly]
        protected bool IsCollider
        {
            get
            {
                if (!Game.IsRunning) { return false; }

                if (IsPoly(gameObject))
                {
                    Material[] materials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                    Material collider = Database.GetAsset<Material>("Collider").Load<Material>();
                    return materials.Contains(collider);
                }

                if (IsShape(gameObject))
                {
                    SpriteShape shape = gameObject.GetComponent<SpriteShapeController>().spriteShape;
                    SpriteShape collider = Database.GetAsset<SpriteShape>("Collider").Load<SpriteShape>();
                    return shape == collider;
                }
                return false;
            }
        }

        [SerializeField, ReadOnly]
        protected bool IsTrigger
        {
            get
            {
                if (!Game.IsRunning) { return false; }

                if (IsPoly(gameObject))
                {
                    Material[] materials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
                    Material trigger = Database.GetAsset<Material>("Trigger").Load<Material>();
                    return materials.Contains(trigger);
                }

                if (IsShape(gameObject))
                {
                    SpriteShape shape = gameObject.GetComponent<SpriteShapeController>().spriteShape;
                    SpriteShape trigger = Database.GetAsset<SpriteShape>("Trigger").Load<SpriteShape>();
                    return shape == trigger;
                }
                return false;
            }
        }

        private List<GameObject> linkedPositions = new();

        private Vector3 positionStored;
        private Vector3 positionDelta;

        private static List<Tag> GetLinkedPositionTags() { return Settings.linkPosition; }

        public static bool IsWorld(GameObject gameObject) { return gameObject.HasComponent<World>(false); }
        public static bool IsPoly(GameObject gameObject) { return gameObject.HasComponent<Poly>(false); }
        public static bool IsShape(GameObject gameObject) { return gameObject.HasComponent<Shape>(false); }

        internal override void TouchLow(GameObject gameObject)
        {
            TryLinkPosition(gameObject);
        }

        internal override void LeaveLow(GameObject gameObject)
        {
            TryUnlinkPosition(gameObject);
        }

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                positionStored = gameObject.transform.position;

                if (hide || IsCollider || IsTrigger)
                {
                    Renderer renderer = GetComponent<Renderer>();
                    renderer.enabled = false;
                }
            }

            base.Awake();
        }

        internal override void FixedUpdate()
        {
            positionDelta = gameObject.transform.position - positionStored;
            UpdateLinkedPositions();
            base.FixedUpdate();
            positionStored = gameObject.transform.position;
        }

        private void TryLinkPosition(GameObject gameObject)
        {
            if (linkedPositions.Contains(gameObject)) { return; }
            if (GetLinkedPositionTags().None(gameObject.HasTag)) { return; }
            linkedPositions.Add(gameObject);
        }

        private void TryUnlinkPosition(GameObject gameObject)
        {
            if (!linkedPositions.Contains(gameObject)) { return; }
            linkedPositions.Remove(gameObject);
        }

        private void UpdateLinkedPositions()
        {
            for (int i = linkedPositions.Count - 1; i >= 0; i--)
            {
                GameObject syncedGameObject = linkedPositions[i];
                if (syncedGameObject == null) { linkedPositions.Remove(syncedGameObject); continue; }
                syncedGameObject.transform.position += positionDelta;
            }
        }

        public static void ApplyColor(GameObject gameObject, Color color)
        {
            if (!IsWorld(gameObject)) { return; }

            if (IsPoly(gameObject))
            {
                Packages.ProBuilder.AccessVertexColor("SetFaceColors", Solver, color);
            }
            if (IsShape(gameObject))
            {
                if (gameObject.TryGetComponent(out SpriteShapeRenderer spriteShape))
                {
                    spriteShape.color = color;
                } 
            }

            if (gameObject.TryGetComponent(out SpriteRenderer sprite)) { sprite.color = color; }
            MemberInfo Solver(MemberInfo[] members) { return members.First(m => ((MethodInfo)m).GetParameters().Any(p => p.ParameterType == typeof(Color))); }
        }

        public static void ApplyMaterial(GameObject gameObject, Material material)
        {
            if (!IsWorld(gameObject)) { return; }

            if (IsPoly(gameObject))
            {
                Packages.ProBuilder.AccessMaterialEditor("ApplyMaterial", null, new List<ProBuilderMesh> { gameObject.GetComponent<ProBuilderMesh>() }, material);
            }
            if (IsShape(gameObject))
            {
                if (gameObject.TryGetComponent(out SpriteShapeRenderer spriteShape))
                {
                    spriteShape.material = material;
                }
            }

            if (gameObject.TryGetComponent(out SpriteRenderer sprite)) { sprite.material = material; }
        }

        public static void MakeTrigger(GameObject gameObject)
        {
            if (!IsWorld(gameObject)) { return; }

            ApplyColor(gameObject, Color.white);

            if (IsPoly(gameObject))
            {
                ApplyMaterial(gameObject, Database.GetAsset<Material>("Trigger").Load<Material>());
            }
            if (IsShape(gameObject))
            {
                SetShapeProfile(gameObject, Database.GetAsset<SpriteShape>("Trigger").Load<SpriteShape>());
            }
            
            if (gameObject.TryGetComponent(out Collider collider))
            {
                if (collider.GetType() == typeof(MeshCollider))
                {
                    ((MeshCollider)collider).convex = true;
                }
                collider.isTrigger = true;
            }
            if (gameObject.TryGetComponent(out Collider2D collider2D))
            {
                collider2D.isTrigger = true;
            }
        }

        public static void MakeCollider(GameObject gameObject)
        {
            if (!IsWorld(gameObject)) { return; }

            ApplyColor(gameObject, Color.white);

            if (IsPoly(gameObject))
            {
                ApplyMaterial(gameObject, Database.GetAsset<Material>("Collider").Load<Material>());
            }
            if (IsShape(gameObject))
            {
                SetShapeProfile(gameObject, Database.GetAsset<SpriteShape>("Collider").Load<SpriteShape>());
                gameObject.GetComponent<SpriteShapeController>().fillPixelsPerUnit = 512;
            }

            if (gameObject.TryGetComponent(out Collider collider))
            {
                collider.isTrigger = false;
                if (collider.GetType() == typeof(MeshCollider))
                {
                    ((MeshCollider)collider).convex = false;
                }
            }

            if (gameObject.TryGetComponent(out Collider2D collider2D))
            {
                collider2D.isTrigger = false;
            }
        }

        public static void SetShapeProfile(GameObject gameObject, SpriteShape spriteShape)
        {
            Member member = new(gameObject.GetComponent<SpriteShapeController>(), "m_SpriteShape");
            member.Set(spriteShape);
        }

        public static T[] SelectionEditor<T>() where T : World
        {
            #if UNITY_EDITOR
            return UnityEditor.Selection.GetFiltered<T>(UnityEditor.SelectionMode.ExcludePrefab);
            #else
            return null;
            #endif
        }
    }
}