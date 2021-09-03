using System;
using System.Linq;
using Jape;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.U2D;

namespace JapeEditor
{
	public class Game : Jape.Game
    {
        [MenuItem("GameObject/Create GameObject", false, -10)]
        public static GameObject EmptyGameObject() { return CreateGameObject(); }

        public new static GameObject CreateGameObject(string name = null, params Type[] components)
        {
            Transform selection = Selection.activeTransform;
            GameObject gameObject = Jape.Game.CreateGameObject(name, components);
            EditorScene.MarkDirty();
            Selection.activeTransform = gameObject.transform;
            if (selection != null)
            {
                gameObject.transform.SetParent(selection, false);
                return gameObject; 
            }
            PositionSceneGameObject(gameObject);
            Prefab.InsertActive(gameObject);
            return gameObject;
        }

        public new static GameObject CloneGameObject(GameObject gameObject, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            Transform selection = Selection.activeTransform;
            GameObject clone = Jape.Game.CloneGameObject(gameObject, position, rotation, parent);
            EditorScene.MarkDirty();
            Selection.activeTransform = clone.transform;
            if (parent != null) { return clone; }
            if (selection != null)
            {
                clone.transform.SetParent(selection, false);
                return clone; 
            }
            if (position == null)
            {
                PositionSceneGameObject(gameObject);
            }
            Prefab.InsertActive(clone);
            return clone;
        }

        public static GameObject ClonePrefab(GameObject gameObject, Transform parent = null)
        {
            Transform selection = Selection.activeTransform;
            GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(gameObject);
            EditorScene.MarkDirty();
            Selection.activeTransform = clone.transform;
            if (parent != null) { return clone; }
            if (selection != null)
            {
                clone.transform.SetParent(selection, false);
                return clone; 
            }
            PositionSceneGameObject(gameObject);
            Prefab.InsertActive(clone);
            return clone;
        }

        private static void PositionSceneGameObject(GameObject gameObject)
        {
            Vector3 position = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f));
            if (SceneView.lastActiveSceneView.in2DMode) { position = new Vector2(position.x, position.y); }
            position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
            gameObject.transform.position = position;
        }

        public static void ColorSelection(Color color)
        {
            foreach (GameObject gameObject in Selection.gameObjects) { World.ApplyColor(gameObject, color); }
        }

        public static void MaterialSelection(Material material)
        {
            foreach (GameObject gameObject in Selection.gameObjects) { World.ApplyMaterial(gameObject, material); }
        }

        public static Poly CreatePoly()
        {
            Transform selection = Selection.activeTransform;
            ProBuilderMesh mesh = Packages.ProBuilder.CreateCube();
            Packages.ProBuilder.InitMesh(mesh);
            GameObject gameObject = mesh.gameObject;
            if (selection != null) { gameObject.transform.SetParent(selection, false); }
            else { PositionSceneGameObject(gameObject); }
            gameObject.AddComponent<Properties>();
            gameObject.AddTag(Tag.Find("World"));
            return gameObject.AddComponent<Poly>();
        }

        public static Shape CreateShape()
        {
            GameObject gameObject = CreateGameObject(null, typeof(PolygonCollider2D), typeof(SpriteShapeRenderer), typeof(SpriteShapeController));
            SpriteShapeRenderer renderer = gameObject.GetComponent<SpriteShapeRenderer>();
            renderer.sharedMaterials = new Material[renderer.sharedMaterials.Length].Select(m => m = DefaultMaterial2D).ToArray();
            World.SetShapeProfile(gameObject, DefaultSpriteShape);
            gameObject.AddTag(Tag.Find("World"));
            return gameObject.AddComponent<Shape>();
        }

        public static SpriteRenderer CreateSprite()
        {
            GameObject gameObject = CreateGameObject(null, typeof(SpriteRenderer));
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = DefaultSprite;
            renderer.material = DefaultMaterial2D;
            return renderer;
        }

        
    }
}