using CustomGraph;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using XNoise;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XNoise_DemoWebglPlayer
{
    public class GraphLibrary : MonoBehaviour
    {
        public static XnoiseGraph CurrentGraph;
        public static Sprite CurrentGraphImage;
        public static Sprite DefaultGradient;
        public static GraphVariables CurrentEditedGraphStorage => CurrentGraph.originalStorage;
        public static event Action<GraphVariables> OnSelectedGraphChanged;

        [Serializable]
        public class GraphAndMetadata
        {
            [SerializeField] public XnoiseGraph graph;
            [SerializeField] public Sprite graphOutputExample;
            [SerializeField] public Sprite graphPreview;
            [SerializeField] public Sprite defaultGradient;
        }

        [SerializeField] private List<GraphAndMetadata> _graphs;
        private UIManager _uiManager;

        private void TriggerSelectedGraphChanged(int arg)
        {
            CurrentGraph = _graphs[arg].graph;
            CurrentGraphImage = _graphs[arg].graphPreview;
            DefaultGradient = _graphs[arg].defaultGradient;
            OnSelectedGraphChanged?.Invoke(_graphs[arg].graph.originalStorage);
        }

        private void Awake()
        {
            _uiManager = GetComponent<UIManager>();
            TriggerSelectedGraphChanged(0);
            FillInGraphTypesData();
            UIManager.SelectedGraphIndexChanged += TriggerSelectedGraphChanged;
        }

        private void OnDestroy()
        {
            UIManager.SelectedGraphIndexChanged -= TriggerSelectedGraphChanged;
        }

        private void FillInGraphTypesData()
        {
            var graphTypeDropdown = _uiManager.getGraphTypeDropdown();

            graphTypeDropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();

            foreach (var graph in _graphs)
            {
                optionDatas.Add(new TMP_Dropdown.OptionData()
                {
#if UNITY_EDITOR
                    text = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(graph.graph)),
#else
                    text = graph.graph.name,
#endif
                    image = graph.graphOutputExample
                });
            }

            graphTypeDropdown.AddOptions(optionDatas);
        }
    }
}