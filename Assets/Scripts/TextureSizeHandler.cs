using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class TextureSizeHandler : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Vector2[] _sizes;

        public Vector2 GetCurrentSizes()
        {
            Vector2 final = Vector2.zero;

            if (_uiManager.textureSize.value == _uiManager.textureSize.options.Count - 1) final = _sizes[_uiManager.textureSize.value];
            else final = new Vector2(float.Parse(_uiManager.customTextureWidth.text), float.Parse(_uiManager.customTextureHeight.text));

            if (_uiManager.plusOne.isOn) final += Vector2.one;

            return final;
        }
    }
}