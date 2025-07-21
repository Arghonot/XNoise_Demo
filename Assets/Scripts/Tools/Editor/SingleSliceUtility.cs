using UnityEngine;
using UnityEditor;
using System.IO;

public class SingleSliceUtility : EditorWindow
{
    [MenuItem("Tools/Slice Selected PNGs As Single Sprites")]
    public static void SliceSelectedPNGs()
    {
        Object[] selectedTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        if (selectedTextures.Length == 0)
        {
            Debug.LogWarning("No textures selected. Please select PNG textures in the Project view.");
            return;
        }

        foreach (Object obj in selectedTextures)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null || !path.ToLower().EndsWith(".png"))
            {
                Debug.LogWarning($"Skipping {obj.name}: not a valid PNG texture.");
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();

            Debug.Log($"Sliced {obj.name} as a single full-image sprite.");
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Remove _resampled from Selected PNG Names")]
    public static void RemoveResampledFromNames()
    {
        var selectedObjects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        foreach (var obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);

            if (filename.Contains("_resampled"))
            {
                string newFilename = filename.Replace("_resampled", "");
                string newPath = Path.Combine(directory, newFilename);

                if (AssetDatabase.RenameAsset(path, newFilename) != "")
                {
                    Debug.LogWarning($"Failed to rename asset: {filename}");
                }
                else
                {
                    Debug.Log($"Renamed: {filename} -> {newFilename}");
                }
            }
        }
    }
}
