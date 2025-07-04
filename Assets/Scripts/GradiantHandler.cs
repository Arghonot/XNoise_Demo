using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class GradiantHandler : MonoBehaviour
    {
        public static Sprite CurrentGradient;

        [SerializeField] private UIManager _uiManager;
        [SerializeField] private Sprite[] _gradients;
        [SerializeField] private Material _objectsMaterial;


        private void Awake()
        {
            SetupGradientDropdown();
            UpdateSelectedGradient(0);
            UIManager.SelectedGradiantIndexChanged += UpdateSelectedGradient;
        }

        private void OnDestroy()
        {
            UIManager.SelectedGradiantIndexChanged -= UpdateSelectedGradient;
        }

        private void SetupGradientDropdown()
        {
            _uiManager.gradiant.ClearOptions();

            var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();

            foreach (var gradient in _gradients)
            {
                if (gradient == null) continue;

                string niceName = gradient.name.Replace("&", " & ").Replace("_0", string.Empty);
                var option = new TMP_Dropdown.OptionData
                {
                    text = niceName,
                    image = gradient
                };

                options.Add(option);
            }

            _uiManager.gradiant.AddOptions(options);
        }

        private void UpdateSelectedGradient(int obj)
        {
            CurrentGradient = _gradients[_uiManager.gradiant.value];
            _objectsMaterial.SetTexture("_Gradient", CurrentGradient.texture);
        }
    }
}