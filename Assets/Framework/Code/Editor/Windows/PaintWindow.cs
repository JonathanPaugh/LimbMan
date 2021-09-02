using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.U2D;
using Jape;

namespace JapeEditor
{
    public class PaintWindow : Window
    {
        protected override string Title => "Paint";

        protected override Display DisplayMode => Display.Popup;

        protected override float MinHeight => 384;
        protected override float Width => 512;

        [PreviewField(128, ObjectFieldAlignment.Center)]
        [HidePicker]
        [HideLabel]
        public Material selectedMaterial;

        [PropertySpace(-2)]

        [HorizontalGroup("Color", 128, MarginLeft = 0.5f, PaddingLeft = -65, PaddingRight = 64)]

        [HideLabel] 
        public Color selectedColor = Color.white;

        [PropertySpace(4)]
        [HorizontalGroup("Alpha", 128, MarginLeft = 0.5f, PaddingLeft = -64, PaddingRight = 64)]

        [ShowInInspector]
        [HideLabel]
        [PropertyRange(0, 1)]
        public float alpha
        {
            get { return selectedColor.a; }
            set { selectedColor.a = value; }
        }
        
        [PropertySpace(16)]
        [HorizontalGroup("Button", 0.5f, MarginLeft = 0.25f)]
        
        [Button(ButtonSizes.Large)] protected void Paint()
        {
            ApplyMaterial(selectedMaterial);
            ApplyColor(selectedColor);
        }

        private static string MaterialPath => Jape.Game.Settings<FrameworkSettings>() != null ? 
                                              $"{Jape.Game.Settings<FrameworkSettings>().gamePath}/System/Resources/MaterialPalettes" : 
                                              string.Empty;                       

        private static string ColorPath => $"{Jape.Game.Settings<FrameworkSettings>().gamePath}/System/Resources/ColorPalettes";

        private static Type MaterialPaletteType => Packages.ProBuilder.MaterialPalette();

        private static ScriptableObject CurrentMaterialPalette => currentMaterialPalette == null ? 
                                                                  GetPalette(currentMaterialPath, MaterialPaletteType) : 
                                                                  currentMaterialPalette;

        private static ScriptableObject currentMaterialPalette;
        private static string currentMaterialPath;

        private ScriptableObject[] availableMaterialPalettes;
        private string[] availableMaterialPaletteNames;
        private int currentMaterialPaletteIndex;

        private static Type ColorPaletteType => Packages.ProBuilder.ColorPalette();

        private static ScriptableObject CurrentColorPalette => currentColorPalette == null ? 
                                                               GetPalette(currentColorPath, ColorPaletteType) : 
                                                               currentColorPalette;

        private static ScriptableObject currentColorPalette;
        private static string currentColorPath;

        private ScriptableObject[] availableColorPalettes;
        private string[] availableColorPaletteNames;
        private int currentColorPaletteIndex;

        private Vector2 materialScroll = Vector2.zero;
        private Vector2 colorScroll = Vector2.zero;

        private static ScriptableObject GetPalette(string path, Type paletteType)
        {
            ScriptableObject palette = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            return palette != null ? palette : Database.GetAsset(null, paletteType).Load<ScriptableObject>();
        }

        protected new void OnEnable()
        {
            if (string.IsNullOrEmpty(currentMaterialPath)) { currentMaterialPath = $"{MaterialPath}/Default.asset"; }
            if (string.IsNullOrEmpty(currentColorPath)) { currentColorPath = $"{ColorPath}/Default.asset"; }

            autoRepaintOnSceneChange = true;

            RefreshMaterialPalettes();
            RefreshColorPalettes();
        }

        protected override void Draw()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            GUILayout.Space(4);

            GUILayout.Label("Materials", EditorStyles.boldLabel.Alignment(TextAnchor.LowerCenter));

            GUILayout.Space(2);

            EditorGUI.BeginChangeCheck();

            currentMaterialPaletteIndex = EditorGUILayout.Popup(GUIContent.none, currentMaterialPaletteIndex, availableMaterialPaletteNames);

            if (EditorGUI.EndChangeCheck())
            {
                ScriptableObject newPalette;
                if (currentMaterialPaletteIndex >= availableMaterialPalettes.Length)
                {
                    string path = $"{MaterialPath}/{Directory.FileIndexName("MaterialPalette", MaterialPath)}.asset";
                    newPalette = CreateInstance(MaterialPaletteType);
                    AssetDatabase.CreateAsset(newPalette, path);
                    EditorGUIUtility.PingObject(newPalette);
                    SetMaterialPalette(newPalette);
                    SetCurrentMaterials(new Material[1]);
                }
                else
                {
                    newPalette = availableMaterialPalettes[currentMaterialPaletteIndex];
                    SetMaterialPalette(newPalette);
                }
            }

            GUILayout.Space(4);

            Material[] materials = GetCurrentMaterials();

            materialScroll = GUILayout.BeginScrollView(materialScroll);

            for (int i = 0; i < materials.Length; i++)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Select", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(48)))
                {
                    SelectMaterial(materials[i]);
                }

                EditorGUI.BeginChangeCheck();

                materials[i] = (Material)EditorGUILayout.ObjectField(materials[i], typeof(Material), false);

                if (EditorGUI.EndChangeCheck())
                {
                    SaveMaterialPalette(materials);
                }

                if (DeleteButton())
                {
                    Material[] temp = new Material[materials.Length - 1];
                    Array.Copy(materials, 0, temp, 0, materials.Length - 1);
                    materials = temp;
                    SaveMaterialPalette(materials);
                    return;
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add"))
            {
                Material[] temp = new Material[materials.Length + 1];
                Array.Copy(materials, 0, temp, 0, materials.Length);
                materials = temp;
                SaveMaterialPalette(materials);
            }

            GUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Colors", EditorStyles.boldLabel.Alignment(TextAnchor.LowerCenter));

            GUILayout.Space(2);

            EditorGUI.BeginChangeCheck();

            currentColorPaletteIndex = EditorGUILayout.Popup(GUIContent.none, currentColorPaletteIndex, availableColorPaletteNames);

            if (EditorGUI.EndChangeCheck())
            {
                ScriptableObject newPalette;
                if (currentColorPaletteIndex >= availableColorPalettes.Length)
                {
                    string path = $"{ColorPath}/{Directory.FileIndexName("ColorPalette", ColorPath)}.asset";
                    newPalette = CreateInstance(ColorPaletteType);
                    AssetDatabase.CreateAsset(newPalette, path);
                    EditorGUIUtility.PingObject(newPalette);
                    SetColorPalette(newPalette);
                    SetCurrentColors(new Color[1]);
                }
                else
                {
                    newPalette = availableColorPalettes[currentColorPaletteIndex];
                    SetColorPalette(newPalette);
                }
            }

            GUILayout.Space(4);

            Color[] colors = GetCurrentColors();

            colorScroll = EditorGUILayout.BeginScrollView(colorScroll);

            for (int i = 0; i < colors.Length; i++)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Select", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(48)))
                {
                    SelectColor(colors[i]);
                }

                colors[i] = EditorGUILayout.ColorField(colors[i]);

                if (DeleteButton())
                {
                    Color[] temp = new Color[colors.Length - 1];
                    Array.Copy(colors, 0, temp, 0, colors.Length - 1);
                    colors = temp;
                    SaveColorPalette(colors);
                    return;
                }

                GUILayout.EndHorizontal();
            }

            if (focusedWindow != null)
            {
                if (!focusedWindow.ToString().Contains("ColorPicker") && !colors.SequenceEqual(GetCurrentColors())) { SaveColorPalette(colors); }
            }

            if (GUILayout.Button("Add"))
            {
                Color[] temp = new Color[colors.Length + 1];
                Array.Copy(colors, 0, temp, 0, colors.Length);
                colors = temp;
                SaveColorPalette(colors);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            bool DeleteButton()
            {
                return GUILayout.Button(Database.GetAsset("IconDelete").Load<Texture>(), 
                                        EditorStyles.miniButton.Offset(new Vector2()).Padding(new RectOffset(1, 1, 1, 1)), 
                                        GUILayout.MaxWidth(18), 
                                        GUILayout.MaxHeight(18));
            }
        }

        private static Material[] GetCurrentMaterials() { return (Material[])Member.Get(currentMaterialPalette, "array"); }
        private static void SetCurrentMaterials(Material[] materials) { Member.Set(currentMaterialPalette, "array", materials); }

        private static void ApplyMaterial(Material material)
        {
            if (material == null) { return; }
            Game.MaterialSelection(material);
        }

        private void SelectMaterial(Material material) { selectedMaterial = material; }

        private void SetMaterialPalette(ScriptableObject palette)
        {
            currentMaterialPalette = palette;
            RefreshMaterialPalettes();
        }

        private void RefreshMaterialPalettes()
        {
            currentMaterialPalette = CurrentMaterialPalette;
            availableMaterialPalettes = Database.GetAssets(null, MaterialPaletteType).Select(r => r.Load<ScriptableObject>()).ToArray();
            availableMaterialPaletteNames = availableMaterialPalettes.Select(m => m.name).ToArray();
            ArrayUtility.Add(ref availableMaterialPaletteNames, string.Empty);
            ArrayUtility.Add(ref availableMaterialPaletteNames, "New Material Palette");
            currentMaterialPaletteIndex = Array.IndexOf(availableMaterialPalettes, currentMaterialPalette);
            currentMaterialPath = AssetDatabase.GetAssetPath(currentMaterialPalette);
        }

        private static void SaveMaterialPalette(Material[] materials)
        {
            SetCurrentMaterials(materials);
            EditorUtility.SetDirty(currentMaterialPalette);
            AssetDatabase.SaveAssets();
        }

        private static Color[] GetCurrentColors() { return ((List<Color>)Member.Get(currentColorPalette, "m_Colors")).ToArray(); }
        private static void SetCurrentColors(Color[] colors) { Member.Set(currentColorPalette, "m_Colors", colors.ToList()); }

        private static void ApplyColor(Color color) { Game.ColorSelection(color); }

        private void SelectColor(Color color) { selectedColor = color; }

        private void SetColorPalette(ScriptableObject palette)
        {
            currentColorPalette = palette;
            RefreshColorPalettes();
        }

        private void RefreshColorPalettes()
        {
            currentColorPalette = CurrentColorPalette;
            availableColorPalettes = Database.GetAssets(null, ColorPaletteType).Select(r => r.Load<ScriptableObject>()).ToArray();
            availableColorPaletteNames = availableColorPalettes.Select(c => c.name).ToArray();
            ArrayUtility.Add(ref availableColorPaletteNames, string.Empty);
            ArrayUtility.Add(ref availableColorPaletteNames, "New Color Palette");
            currentColorPaletteIndex = Array.IndexOf(availableColorPalettes, currentColorPalette);
            currentColorPath = AssetDatabase.GetAssetPath(currentColorPalette);
        }

        private static void SaveColorPalette(Color[] colors)
        {
            SetCurrentColors(colors);
            EditorUtility.SetDirty(currentColorPalette);
            AssetDatabase.SaveAssets();
        }
    }
}