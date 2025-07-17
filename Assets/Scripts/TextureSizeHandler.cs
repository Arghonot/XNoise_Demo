using System;
using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class TextureSizeHandler : MonoBehaviour
    {
        public static event Action OnTextureSizeChanged;
        public static Vector2 CurrentResolution = Vector2.one;

        private Vector2 currentResolutionWithoutPlusOne = Vector2.one;

        private UIManager _uiManager;
        [SerializeField] private float[] _sizes;

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();

            UIManager.SelectedTextureSizeIndexChanged += UpdateResolution;
            UIManager.PlusOneStateChanged += UpdateResolution;
            UIManager.TextureHeightOrWidthChanged += HandleTextureSizeChanged;

            CurrentResolution = GetCurrentSize();
        }

        private void OnDestroy()
        {
            UIManager.SelectedTextureSizeIndexChanged -= UpdateResolution;
            UIManager.PlusOneStateChanged -= UpdateResolution;
            UIManager.TextureHeightOrWidthChanged -= HandleTextureSizeChanged;
        }

        private void UpdateResolution(bool obj) => UpdateResolution();
        private void UpdateResolution(int obj) => UpdateResolution();
        private void UpdateResolution()
        {
            CurrentResolution = GetCurrentSize();
        }

        public static float GetFloatFromInputField(TMP_InputField inputField)
        {
            float value;
            if (float.TryParse(inputField.text, out value)) return value;
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

        private void HandleTextureSizeChanged()
        {
            var newRes = UpdateResolutionBasedOnInputFields();
            if (newRes.x < 1 || newRes.x > 4096 || newRes.y < 1 || newRes.y > 4096)
            {
                ResetInputFields();
                return;
            }
            CurrentResolution = newRes;
            OnTextureSizeChanged?.Invoke();
        }

        public Vector2 GetCurrentSize()
        {
            Vector2 final = Vector2.zero;

            _uiManager.customTextureDimensionHolder.SetActive(false);
            _uiManager.textureDimensionLabel.SetActive(true);
            if (_uiManager.textureSize.value < _uiManager.textureSize.options.Count - 1) final = Vector2.one * _sizes[_uiManager.textureSize.value];
            else
            {
                ResetInputFields();
                _uiManager.customTextureDimensionHolder.SetActive(true);
                _uiManager.textureDimensionLabel.SetActive(false);
                final = UpdateResolutionBasedOnInputFields();
            }

            if (_uiManager.plusOne.isOn) final += Vector2.one;

            return final;
        }

        private void ResetInputFields()
        {
            int width = (int)CurrentResolution.x;
            int height = (int)CurrentResolution.y;
            if (!_uiManager.plusOne.isOn)
            {
                width -= 1;
                height -= 1;
            }
            _uiManager.customTextureWidth.SetTextWithoutNotify(width.ToString());
            _uiManager.customTextureHeight.SetTextWithoutNotify(height.ToString());
        }
    }
}