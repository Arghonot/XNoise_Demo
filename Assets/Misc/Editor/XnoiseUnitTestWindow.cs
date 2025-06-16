using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using XNode;
using static XNode.Node;
using Unity.Plastic.Newtonsoft.Json;
using XNoise;

namespace CustomUnitTesting
{
    public class XnoiseUnitTestWindow : EditorWindow
    {
        [MenuItem("Window/Xnoise/Run Unit Tests")]
        public static void ShowWindow()
        {
            GetWindow<XnoiseUnitTestWindow>("Xnoise Tests");
        }

        private NodeGraph _selectedGraph;

        private void OnGUI()
        {
            GUILayout.Label("Xnoise Unit Test Utilities", EditorStyles.boldLabel);

            _selectedGraph = EditorGUILayout.ObjectField("Selected Graph", _selectedGraph, typeof(NodeGraph), true) as NodeGraph;

            if (GUILayout.Button("Export Selected Nodes to JSON"))
            {
                if (_selectedGraph == null)
                {
                    Debug.LogError("No graph selected.");
                    return;
                }
                XNoiseGraphSelectionSaverLoader.ExportSingleGraphToJson(_selectedGraph);
            }

            if (GUILayout.Button("Import JSON to New Graph"))
            {
                var openPath = UnityEditor.EditorUtility.OpenFilePanel("Load Graph JSON", Application.dataPath, "json");
                var savePath = UnityEditor.EditorUtility.SaveFolderPanel("Select Folder To Save Graph", Application.dataPath, "");
                XNoiseGraphSelectionSaverLoader.ImportSingleGraphFromJson(openPath, savePath);
            }
            GUILayout.Space(42f);
            if (GUILayout.Button("Run Unit Tests from Folder"))
            {
                var sourceDir = EditorUtility.OpenFolderPanel("Select JSON Root Folder", Application.dataPath, "");
                if (string.IsNullOrEmpty(sourceDir)) return;
                var targetDir = EditorUtility.SaveFolderPanel("Select Output Folder", Application.dataPath, "TestResults");
                if (string.IsNullOrEmpty(targetDir)) return;

                XNoiseJsonUnitTester.RunTestsFromFolder(sourceDir, targetDir);
            }
        }
    }
}