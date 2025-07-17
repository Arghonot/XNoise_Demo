using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class GradientToTexture : EditorWindow
    {
        private Gradient gradient = new Gradient();
        private int width = 256;
        private string textureName = "GradientTexture";
        private string gradientMetadata = "Metadata";
        private List<Texture2D> _textures;

        [MenuItem("Tools/Generate Gradient Texture")]
        public static void ShowWindow()
        {
            GetWindow<GradientToTexture>("Gradient Texture Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Gradient Texture Generator", EditorStyles.boldLabel);

            gradient = EditorGUILayout.GradientField("Gradient", gradient);
            width = EditorGUILayout.IntField("Texture Width", width);
            textureName = EditorGUILayout.TextField("Texture Name", textureName);
            gradientMetadata = EditorGUILayout.TextField("Texture Name", gradientMetadata);
            var selected = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            _textures = new List<Texture2D>(selected);

            if (GUILayout.Button("Generate & Save"))
            {
                GenerateGradientTexture();
            }

            if (GUILayout.Button("Generate gradient from metadata"))
            {
                gradient = CreateGradientFromHexString(gradientMetadata);
            }
            if (GUILayout.Button("Slice textures"))
            {
                SetTexturesAsSlicedSprites(_textures);
            }
        }

        /// <summary>
        /// Takes a list of Texture2D assets and sets them to Sprite (2D and UI) with Sprite Mode = Single and Sliced border.
        /// </summary>
        public static void SetTexturesAsSlicedSprites(List<Texture2D> textures, Vector4 border = default)
        {
            if (textures == null || textures.Count == 0)
            {
                Debug.LogWarning("No textures provided.");
                return;
            }

            foreach (var texture in textures)
            {
                if (texture == null) continue;

                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 100;
                    importer.spriteBorder = border; // e.g., new Vector4(10,10,10,10)

                    // For slicing: Full Rect mesh type is default for Single mode.
                    //importer.spriteMeshType = SpriteMeshType.FullRect;

                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();

                    Debug.Log($"Set '{texture.name}' as sliced sprite with border {border}.");
                }
                else
                {
                    Debug.LogWarning($"Could not get importer for texture at {path}");
                }
            }

            AssetDatabase.Refresh();
        }

        public static Gradient CreateGradientFromHexString(string hexString)
        {
            string[] hexColors = hexString.Split(' ');
            int count = hexColors.Length;

            GradientColorKey[] colorKeys = new GradientColorKey[count];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[count];

            for (int i = 0; i < count; i++)
            {
                if (ColorUtility.TryParseHtmlString(hexColors[i], out Color color))
                {
                    float time = Mathf.Lerp(0f, 1f, i / (float)(count - 1));
                    colorKeys[i] = new GradientColorKey(color, time);
                    alphaKeys[i] = new GradientAlphaKey(color.a, time);
                }
                else
                {
                    Debug.LogWarning($"Invalid hex: {hexColors[i]}");
                }
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private void GenerateGradientTexture()
        {
            if (width <= 0)
            {
                Debug.LogError("Width must be greater than zero.");
                return;
            }

            Texture2D texture = new Texture2D(width, 1, TextureFormat.RGBA32, false);

            for (int x = 0; x < width; x++)
            {
                float t = (float)x / (width - 1);
                Color color = gradient.Evaluate(t);
                texture.SetPixel(x, 0, color);
            }

            texture.Apply();

            // Save as PNG
            byte[] pngData = texture.EncodeToPNG();
            string path = $"Assets/{textureName}.png";
            File.WriteAllBytes(path, pngData);
            Debug.Log($"Gradient texture saved to {path}");

            // Refresh AssetDatabase so it shows up
            AssetDatabase.Refresh();
        }
    }
}