namespace XNoise_DemoWebglPlayer
{
    public class DownloadToastMessageBehaviour : ToastBehaviourBase
    {
        void Start() => UIManager.downloadButtonClicked += ShowToast;
        private void OnDestroy() => UIManager.downloadButtonClicked -= ShowToast;
    }
}