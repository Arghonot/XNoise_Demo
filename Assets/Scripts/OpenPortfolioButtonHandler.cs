using UnityEngine;
namespace XNoise_DemoWebglPlayer
{
    public class OpenPortfolioButtonHandler : MonoBehaviour
    {
        void Start() => UIManager.openPortfolioButtonClicked += OnOpenPortfolioButtonClicked;
        private void OnDestroy() => UIManager.openPortfolioButtonClicked += OnOpenPortfolioButtonClicked;
        private void OnOpenPortfolioButtonClicked() => Application.OpenURL("https://loick.rivemale.space/");
    }
}