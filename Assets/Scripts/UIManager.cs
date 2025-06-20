using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _graphType;
        [SerializeField] private TMP_Dropdown _textureSize;
        [SerializeField] private Toggle _plusOne;
        [SerializeField] private TMP_Dropdown _projectionType;
        [SerializeField] private TMP_Dropdown _gradiant;
        [SerializeField] private TMP_Dropdown _visualizerObject;
        [SerializeField] private Button _generate;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _copySeed;
        [SerializeField] private Toggle _renderView;
        [SerializeField] private Toggle _graphView;
        [SerializeField] private Toggle _randomSeed;
        [SerializeField] private TMP_InputField _seed;
        [SerializeField] private TMP_InputField _customTextureWidth;
        [SerializeField] private TMP_InputField _customTextureHeight;
        [SerializeField] private GameObject _customTextureDimensionHolder;

        public static event UnityAction SaveButtonClicked;
        public static event UnityAction GenerateButtonClicked;
        public static event UnityAction CopyButtonClicked;

        public static event UnityAction<bool> PlusOneStateChanged;
        public static event UnityAction<bool> RandomSeedStateChanged;
        public static event UnityAction<bool> RenderSceneStateChanged; // graph or 3D scene render ?

        public static event UnityAction<int> SelectedGraphIndexChanged;
        public static event UnityAction<int> SelectedTextureSizeIndexChanged;
        public static event UnityAction<int> SelectedProjectionTypeIndexChanged;
        public static event UnityAction<int> SelectedGradiantIndexChanged;
        public static event UnityAction<int> SelectedSceneObjectChanged;

        public static event Action<string, object> InputValueChanged;


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
            //_graphType.onValueChanged.AddListener(SelectedGraphIndexChanged);
            _textureSize.onValueChanged.AddListener(SelectedTextureSizeIndexChanged);
            _projectionType.onValueChanged.AddListener(SelectedProjectionTypeIndexChanged);
            _gradiant.onValueChanged.AddListener(SelectedGradiantIndexChanged);
            _visualizerObject.onValueChanged.AddListener(SelectedSceneObjectChanged);
        }

        private void UnregisterEvents()
        {
            _graphType.onValueChanged.AddListener(SelectedGraphIndexChanged);
            _textureSize.onValueChanged.AddListener(SelectedTextureSizeIndexChanged);
            _projectionType.onValueChanged.AddListener(SelectedProjectionTypeIndexChanged);
            _gradiant.onValueChanged.AddListener(SelectedGradiantIndexChanged);
            _visualizerObject.onValueChanged.RemoveListener(SelectedSceneObjectChanged);
        }
    }
}