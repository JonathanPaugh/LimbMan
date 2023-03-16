using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Jape;

namespace JapeEditor
{
    public class ElementWindow : Window
    {
        protected override string Title => "Create Element";

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;

        protected override float Width => 512;

        [ValueDropdown(nameof(GetElementTypes))]
        [ListDrawerSettings(HideAddButton = true, Expanded = true)] 
        public List<Type> elements = new(new Type[] { null });
        private IEnumerable<Type> GetElementTypes() { return Element.Subclass(); }

        [Button(ButtonSizes.Large)]
        private void Add() { elements.Add(null); }

        protected override void Draw()
        {
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create", GUILayout.Height(32))) { Game.CreateGameObject(null, elements.Select(e => e).ToArray()); }

            if (Selection.activeGameObject == null) { GUI.enabled = false; }

            if (GUILayout.Button($"Attach {Selection.gameObjects.Length}", GUILayout.Height(32)))
            {
                if (Selection.activeGameObject != null) 
                {
                    foreach (GameObject gameObject in Selection.gameObjects)
                    {
                        foreach (Type element in elements)
                        {
                            gameObject.AddComponent(element);
                        }
                    }
                }
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        [MenuItem("GameObject/Create Element", false, -9)]
        private static void Menu() { Open<ElementWindow>(); }
    }
}