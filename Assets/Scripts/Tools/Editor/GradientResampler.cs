using UnityEngine;
using UnityEditor;
using System.IO;

public class GradientResampler : EditorWindow
{
    private Object[] selectedSprites;
    private int outputWidth = 2048;

    [MenuItem("Tools/Resample and Slice Gradients")]
    public static void ShowWindow()
    {
        GetWindow<GradientResampler>("Gradient Resampler");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select 512x1 gradient textures in Project view", EditorStyles.wordWrappedLabel);
        outputWidth = EditorGUILayout.IntField("Output Width", outputWidth);

        if (GUILayout.Button("Resample and Slice Selected Gradients"))
        {
            selectedSprites = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            if (selectedSprites.Length == 0)
            {
                Debug.LogWarning("No gradient textures selected.");
                return;
            }

            foreach (Object obj in selectedSprites)
            {
                Texture2D sourceTex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(obj));
                if (sourceTex == null || sourceTex.height != 1)
                {
                    Debug.LogWarning($"Skipping {obj.name}: not a valid 1-pixel height texture.");
                    continue;
                }

                string path = GenerateAndSaveGradient(sourceTex);
                SliceAsSingleSprite(path);
            }

            AssetDatabase.Refresh();
        }
    }

    private string GenerateAndSaveGradient(Texture2D sourceTex)
    {
        // Ensure texture is readable
        string path = AssetDatabase.GetAssetPath(sourceTex);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (!importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        int width = sourceTex.width;
        Color[] colors = new Color[6];
        for (int i = 0; i < 6; i++)
        {
            int x = Mathf.Clamp(Mathf.RoundToInt((i / 5f) * (width - 1)), 0, width - 1);
            colors[i] = sourceTex.GetPixel(x, 0);
        }

        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[6];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[6];

        for (int i = 0; i < 6; i++)
        {
            float time = i / 5f;
            colorKeys[i] = new GradientColorKey(colors[i], time);
            alphaKeys[i] = new GradientAlphaKey(colors[i].a, time);
        }

        gradient.SetKeys(colorKeys, alphaKeys);

        Texture2D newTex = new Texture2D(outputWidth, 1, TextureFormat.RGBA32, false);
        for (int x = 0; x < outputWidth; x++)
        {
            float t = x / (float)(outputWidth - 1);
            newTex.SetPixel(x, 0, gradient.Evaluate(t));
        }
        newTex.Apply();

        string exportPath = Path.GetDirectoryName(path) + "/" + sourceTex.name + "_resampled.png";
        File.WriteAllBytes(exportPath, newTex.EncodeToPNG());
        Debug.Log($"Saved and resampled: {exportPath}");

        return exportPath;
    }

    private void SliceAsSingleSprite(string texturePath)
    {
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.isReadable = true;
        importer.mipmapEnabled = false;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();

        Debug.Log($"Sliced as single sprite: {texturePath}");
    }
}
