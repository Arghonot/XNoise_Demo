namespace XNoise_DemoWebglPlayer
{
    public class CopiedSeedToastMessageBehaviour : ToastBehaviourBase
    {
        void Start() => SeedRowHandler.OnCopiedSeedToClipboard += ShowToast;
        private void OnDestroy() => SeedRowHandler.OnCopiedSeedToClipboard -= ShowToast;
    }
}