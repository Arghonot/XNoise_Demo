using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class TextureSizeHandler : MonoBehaviour
    {
        public static Vector2 CurrentResolution = Vector2.one;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private float[] _sizes;

        private void Awake()
        {
            UIManager.SelectedTextureSizeIndexChanged += UpdateResolution;
            UIManager.PlusOneStateChanged += UpdateResolution;

            CurrentResolution = GetCurrentSize();
        }

        private void OnDestroy()
        {
            UIManager.SelectedTextureSizeIndexChanged -= UpdateResolution;
            UIManager.PlusOneStateChanged -= UpdateResolution;

        }

        private void UpdateResolution(bool obj) => CurrentResolution = GetCurrentSize();
        private void UpdateResolution(int obj) => CurrentResolution = GetCurrentSize();

        public Vector2 GetCurrentSize()
        {
            Vector2 final = Vector2.zero;

            if (_uiManager.textureSize.value < _uiManager.textureSize.options.Count - 1) final = Vector2.one * _sizes[_uiManager.textureSize.value];
            else final = new Vector2(float.Parse(_uiManager.customTextureWidth.text), float.Parse(_uiManager.customTextureHeight.text));

            if (_uiManager.plusOne.isOn) final += Vector2.one;

            return final;
        }
    }
}