using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class TextureSizeHandler : MonoBehaviour
    {
        public static Vector2 CurrentResolution = Vector2.one;
        private UIManager _uiManager;
        [SerializeField] private float[] _sizes;

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();

            UIManager.TextureHeightOrWidthChanged += UpdateResolution;
            UIManager.SelectedTextureSizeIndexChanged += UpdateResolution;
            UIManager.PlusOneStateChanged += UpdateResolution;

            CurrentResolution = GetCurrentSize();
        }

        private void OnDestroy()
        {
            UIManager.SelectedTextureSizeIndexChanged -= UpdateResolution;
            UIManager.PlusOneStateChanged -= UpdateResolution;
        }

        private void UpdateResolution(bool obj) => UpdateResolution();
        private void UpdateResolution(int obj) => UpdateResolution();
        private void UpdateResolution() => CurrentResolution = GetCurrentSize();

        public static float GetFloatFromInputField(TMP_InputField inputField)
        {
            float value;
            if (float.TryParse(inputField.text, out value))
            {
                return value;
            }
            else
            {
                Debug.LogWarning($"InputField '{inputField.name}' does not contain a valid float: '{inputField.text}'");
                return 0f;
            }
        }

        private Vector2 UpdateResolutionBasedOnInputFields()
        {
            var newRes = new Vector2(GetFloatFromInputField(_uiManager.customTextureWidth), GetFloatFromInputField(_uiManager.customTextureHeight));
            return newRes;
        }

        public Vector2 GetCurrentSize()
        {
            Vector2 final = Vector2.zero;

            _uiManager.customTextureDimensionHolder.SetActive(false);
            print(_uiManager.textureSize.options.Count + " " + _uiManager.textureSize.value);
            if (_uiManager.textureSize.value < _uiManager.textureSize.options.Count - 1) final = Vector2.one * _sizes[_uiManager.textureSize.value];
            else
            {
                _uiManager.customTextureDimensionHolder.SetActive(true);
                final = UpdateResolutionBasedOnInputFields();
            }

            if (_uiManager.plusOne.isOn) final += Vector2.one;

            return final;
        }
    }
}