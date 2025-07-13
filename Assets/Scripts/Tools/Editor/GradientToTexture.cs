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

            if (GUILayout.Button("Generate & Save"))
            {
                GenerateGradientTexture();
            }
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