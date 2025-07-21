using CustomGraph;
using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class GradiantHandler : MonoBehaviour
    {
        public static Sprite CurrentGradient;

        private UIManager _uiManager;
        [SerializeField] private Sprite[] _gradients;

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();
            SetupGradientDropdown();
            UpdateSelectedGradient(0);
            UIManager.SelectedGradiantIndexChanged += UpdateSelectedGradient;
            GraphLibrary.OnSelectedGraphChanged += UseNewGraphDefaultGradient;
        }

        private void OnDestroy()
        {
            UIManager.SelectedGradiantIndexChanged -= UpdateSelectedGradient;
            GraphLibrary.OnSelectedGraphChanged -= UseNewGraphDefaultGradient;
        }

        private void UseNewGraphDefaultGradient(GraphVariables variables)
        {
            if (GraphLibrary.CurrentGraph == null || GraphLibrary.DefaultGradient == null) return;
            _uiManager.gradiant.value = GetGradientIndex(GraphLibrary.DefaultGradient);
            UpdateSelectedGradient(_uiManager.gradiant.value);
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

        private void UpdateSelectedGradient(int obj) => CurrentGradient = _gradients[_uiManager.gradiant.value];

        public int GetGradientIndex(Sprite gradient)
        {
            for (int i = 0; i < _gradients.Length; i++)
                if (_gradients[i] == gradient)
                    return i;
            return -1;
        }
    }
}