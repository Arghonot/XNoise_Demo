using UnityEngine;

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
            SaveRenderTexture(GraphLibrary.CurrentGraph.renderer.rtex);
#else
            GraphLibrary.CurrentGraph.renderer.Save(GetImageName(GraphLibrary.CurrentGraph.renderer.rtex));
#endif
        }

        public void SaveRenderTexture(RenderTexture rtex)
        {
            string fileName = GetImageName(rtex);
            Texture2D tex = new Texture2D(rtex.width, rtex.height, TextureFormat.RGBA32, false);
            RenderTexture.active = rtex;
            tex.ReadPixels(new Rect(0, 0, rtex.width, rtex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

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
            foreach (var item in GraphLibrary.CurrentEditedGraphStorage)
            {
                if (item.Name == "Seed") continue;
                result += $"_{item.Name}_{item.GetValue()}";
            }
            return result;
        }

        private string GetImageName(RenderTexture rtex)
        {
            string graphName = GraphLibrary.CurrentGraph.name;
            string textureSize = $"{rtex.width.ToString()}x{rtex.height.ToString()}";
            string seed = SeedRowHandler.Seed;
            return $"{graphName}_{textureSize}_{seed}__{GetAllArgumentsValues()}.png";
        }
    }
}