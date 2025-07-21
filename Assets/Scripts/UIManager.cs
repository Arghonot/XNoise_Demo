using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _graphType;
        public TMP_Dropdown graphType => _graphType;
        [SerializeField] private TMP_Dropdown _textureSize;
        public TMP_Dropdown textureSize => _textureSize;
        [SerializeField] private Toggle _plusOne;
        public Toggle plusOne => _plusOne;
        [SerializeField] private TMP_Dropdown _projectionType;
        public TMP_Dropdown projectionType => _projectionType;
        [SerializeField] private TMP_Dropdown _gradiant;
        public TMP_Dropdown gradiant => _gradiant;
        [SerializeField] private TMP_Dropdown _visualizerObject;
        public TMP_Dropdown visualizerObject => _visualizerObject;
        [SerializeField] private Button _openPortfolio;
        public Button openPortfolio => _openPortfolio;
        [SerializeField] private Button _generate;
        public Button generate => _generate;
        [SerializeField] private Button _saveButton;
        public Button saveButton => _saveButton;
        [SerializeField] private Button _copySeed;
        public Button copySeed => _copySeed;
        [SerializeField] private Toggle _renderView;
        public Toggle renderView => _renderView;
        [SerializeField] private Toggle _graphView;
        public Toggle graphView => _graphView;
        [SerializeField] private Toggle _randomSeed;
        public Toggle randomSeed => _randomSeed;
        [SerializeField] private TMP_InputField _seed;
        public TMP_InputField seed => _seed;
        [SerializeField] private TMP_InputField _customTextureWidth;
        public TMP_InputField customTextureWidth => _customTextureWidth;
        [SerializeField] private TMP_InputField _customTextureHeight;
        public TMP_InputField customTextureHeight => _customTextureHeight;
        [SerializeField] private GameObject _customTextureDimensionHolder;
        public GameObject customTextureDimensionHolder => _customTextureDimensionHolder;
        [SerializeField] private GameObject _textureDimensionLabel;
        public GameObject textureDimensionLabel => _textureDimensionLabel;

        public static event Action openPortfolioButtonClicked;
        public static event Action downloadButtonClicked;
        public static event Action GenerateButtonClicked;
        public static event Action CopyButtonClicked;
        public static event Action TextureHeightOrWidthChanged;

        public static event Action<bool> PlusOneStateChanged;
        public static event Action<bool> RandomSeedStateChanged;
        public static event Action<bool> RenderSceneStateChanged; // graph or 3D scene render ?

        public static event Action<int> SelectedGraphIndexChanged;
        public static event Action<int> SelectedTextureSizeIndexChanged;
        public static event Action<int> SelectedProjectionTypeIndexChanged;
        public static event Action<int> SelectedGradiantIndexChanged;
        public static event Action<int> SelectedSceneObjectChanged;

        private void TriggerOpenPortfolioButtonClicked() => openPortfolioButtonClicked?.Invoke();
        private void TriggerSaveButtonClicked() => downloadButtonClicked?.Invoke();
        private void TriggerGenerateButtonClicked() => GenerateButtonClicked?.Invoke();
        private void TriggerCopyButtonClicked() => CopyButtonClicked?.Invoke();
        private void TriggerTextureHeightOrWidthChanged(string arg) => TextureHeightOrWidthChanged?.Invoke();

        private void TriggerPlusOneStateChanged(bool arg) => PlusOneStateChanged?.Invoke(arg);
        private void TriggerRandomSeedStateChanged(bool arg) => RandomSeedStateChanged?.Invoke(arg);
        private void TriggerRenderSceneStateChanged(bool arg) => RenderSceneStateChanged?.Invoke(arg);

        private void TriggerSelectedGraphIndexChanged(int arg) => SelectedGraphIndexChanged?.Invoke(arg);
        private void TriggerSelectedTextureSizeIndexChanged(int arg) => SelectedTextureSizeIndexChanged?.Invoke(arg);
        private void TriggerSelectedProjectionTypeIndexChanged(int arg) => SelectedProjectionTypeIndexChanged?.Invoke(arg);
        private void TriggerSelectedGradiantIndexChanged(int arg) => SelectedGradiantIndexChanged?.Invoke(arg);
        private void TriggerSelectedSceneObjectChanged(int arg) => SelectedSceneObjectChanged?.Invoke(arg);


        public TMP_Dropdown getGraphTypeDropdown() => _graphType;

        private void Awake()
        {
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            _openPortfolio.onClick.AddListener(TriggerOpenPortfolioButtonClicked);
            _saveButton.onClick.AddListener(TriggerSaveButtonClicked);
            _generate.onClick.AddListener(TriggerGenerateButtonClicked);
            _copySeed.onClick.AddListener(TriggerCopyButtonClicked);
            _plusOne.onValueChanged.AddListener(TriggerPlusOneStateChanged);
            _randomSeed.onValueChanged.AddListener(TriggerRandomSeedStateChanged);
            _renderView.onValueChanged.AddListener(TriggerRenderSceneStateChanged);
            _graphType.onValueChanged.AddListener(TriggerSelectedGraphIndexChanged);
            _textureSize.onValueChanged.AddListener(TriggerSelectedTextureSizeIndexChanged);
            _projectionType.onValueChanged.AddListener(TriggerSelectedProjectionTypeIndexChanged);
            _gradiant.onValueChanged.AddListener(TriggerSelectedGradiantIndexChanged);
            _visualizerObject.onValueChanged.AddListener(TriggerSelectedSceneObjectChanged);
            _customTextureWidth.onEndEdit.AddListener(TriggerTextureHeightOrWidthChanged);
            _customTextureHeight.onEndEdit.AddListener(TriggerTextureHeightOrWidthChanged);
        }

        private void UnregisterEvents()
        {
            _openPortfolio.onClick.RemoveListener(TriggerOpenPortfolioButtonClicked);
            _saveButton.onClick.RemoveListener(TriggerSaveButtonClicked);
            _generate.onClick.RemoveListener(TriggerGenerateButtonClicked);
            _copySeed.onClick.RemoveListener(TriggerCopyButtonClicked);
            _plusOne.onValueChanged.RemoveListener(TriggerPlusOneStateChanged);
            _randomSeed.onValueChanged.RemoveListener(TriggerRandomSeedStateChanged);
            _renderView.onValueChanged.RemoveListener(TriggerRenderSceneStateChanged);
            _graphType.onValueChanged.RemoveListener(TriggerSelectedGraphIndexChanged);
            _textureSize.onValueChanged.RemoveListener(TriggerSelectedTextureSizeIndexChanged);
            _projectionType.onValueChanged.RemoveListener(TriggerSelectedProjectionTypeIndexChanged);
            _gradiant.onValueChanged.RemoveListener(TriggerSelectedGradiantIndexChanged);
            _visualizerObject.onValueChanged.RemoveListener(TriggerSelectedSceneObjectChanged);
            _customTextureWidth.onEndEdit.RemoveListener(TriggerTextureHeightOrWidthChanged);
            _customTextureHeight.onEndEdit.RemoveListener(TriggerTextureHeightOrWidthChanged);
        }
    }
}