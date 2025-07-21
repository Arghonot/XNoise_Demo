using CustomGraph;
using System.Runtime.InteropServices;
using UnityEngine;
using XNoise;

namespace XNoise_DemoWebglPlayer
{
    public class TextureDownloadHandler : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string base64, string fileName);
#endif

        void Awake()
        {
            UIManager.downloadButtonClicked += WriteFileOnDevice;
        }

        private void OnDestroy()
        {
            UIManager.downloadButtonClicked -= WriteFileOnDevice;
        }

        private void WriteFileOnDevice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SaveRenderTexture(ImageFileHelpers.BlitMaterialToTexture(GraphRendererHandler.OriginalRenderGradient2d, (int)TextureSizeHandler.CurrentResolution.x, (int)TextureSizeHandler.CurrentResolution.y));
#else
            GraphLibrary.CurrentGraph.renderer.Save(ImageFileHelpers.BlitMaterialToTexture(GraphRendererHandler.OriginalRenderGradient2d), GetImageName(GraphLibrary.CurrentGraph.renderer.rtex));
#endif
        }

        public void SaveRenderTexture(Texture2D tex)
        {
            string fileName = GetImageName(tex);

            byte[] pngBytes = tex.EncodeToPNG();
            Destroy(tex);

            string base64 = System.Convert.ToBase64String(pngBytes);

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadFile(base64, fileName);
#else
            // For desktop dev: save to file system
            System.IO.File.WriteAllBytes(Application.dataPath + "/" + fileName, pngBytes);
            Debug.Log("Saved to " + Application.dataPath + "/" + fileName);
#endif
        }

        private string GetAllArgumentsValues()
        {
            string result = string.Empty;
            foreach (var item in GraphArgumentsHandler.currentStorage)
            {
                if (item.Name == "Seed") continue;
                result += $"_{item.Name}_{item.GetValue()}";
            }
            return result;
        }

        private string GetImageName(RenderTexture rtex)
        {
            return GetImageName(GraphLibrary.CurrentGraph.name, new Vector2(rtex.width, rtex.height));
        }

        private string GetImageName(Texture2D tex)
        {
            return GetImageName(GraphLibrary.CurrentGraph.name, new Vector2(tex.width, tex.height));
        }

        private string GetImageName(string graphName, Vector2 texSize)
        {
            string textureSize = $"{texSize.x.ToString()}x{texSize.y.ToString()}";
            string seed = SeedRowHandler.Seed;
            return $"{graphName}_{textureSize}_[Seed]_{seed}__{GetAllArgumentsValues()}.png";
        }
    }
}