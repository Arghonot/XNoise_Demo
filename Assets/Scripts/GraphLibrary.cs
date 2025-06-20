using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using XNoise;

namespace XNoise_DemoWebglPlayer
{
    public class GraphLibrary : MonoBehaviour
    {
        [Serializable]
        public class GraphAndMetadata
        {
            [SerializeField] public XnoiseGraph graph;
            [SerializeField] public Sprite graphOutputExample;
            [SerializeField] public Sprite graphPreview;
        }

        [SerializeField] private List<GraphAndMetadata> _graphs;
        [SerializeField] private UIManager _UIManager;

        private void Awake()
        {
            FillInGraphTypesData();
        }

        private void FillInGraphTypesData()
        {
            var graphTypeDropdown = _UIManager.getGraphTypeDropdown();

            graphTypeDropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();

            foreach (var graph in _graphs)
            {
                optionDatas.Add(new TMP_Dropdown.OptionData()
                {
                    text = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(graph.graph)),
                    image = graph.graphOutputExample
                });
            }

            graphTypeDropdown.AddOptions(optionDatas);
        }
    }
}