using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class GradiantHandler : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Texture2D[] _gradients;

        public Texture2D GetCurrentGradient() => _gradients[_uiManager.gradiant.value];
    }
}