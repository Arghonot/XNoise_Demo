using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json; // You need Newtonsoft.Json package installed
using XNode;
using static XNode.Node;
using Unity.Plastic.Newtonsoft.Json;

namespace CustomUnitTesting
{
    [Serializable]
    public class GraphExport
    {
        public List<NodeExport> nodes = new List<NodeExport>();
        public List<NodeConnectionExport> connections = new List<NodeConnectionExport>();
    }

    [Serializable]
    public class NodeExport
    {
        public string id;
        public int guid;
        public string type;
        public Dictionary<string, object> inputs;
    }

    [Serializable]
    public class NodeConnectionExport
    {
        public int fromNodeGuid;
        public int toNodeGuid;
        public string inputField;
    }
    public class XnoiseUnitTestWindow : EditorWindow
    {
        [MenuItem("Window/Xnoise/Run Unit Tests")]
        public static void ShowWindow()
        {
            GetWindow<XnoiseUnitTestWindow>("Xnoise Tests");
        }

        private XnoiseBasicUnitTests testRunner;
        private NodeGraph selectedGraph;

        private void OnGUI()
        {
            GUILayout.Label("Xnoise Unit Test Utilities", EditorStyles.boldLabel);

            if (GUILayout.Button("Run Perlin CPU Test"))
            {
                EnsureRunner();
                testRunner.TestPerlinCPU();
            }

            if (GUILayout.Button("Run Graph Order Test"))
            {
                EnsureRunner();
                testRunner.TestGraphOrder();
            }

            if (GUILayout.Button("Run GPU vs CPU Comparison"))
            {
                EnsureRunner();
                testRunner.TestGPUvsCPU();
            }

            EditorGUILayout.Space();

            selectedGraph = EditorGUILayout.ObjectField("Selected Graph", selectedGraph, typeof(NodeGraph), true) as NodeGraph;

            if (GUILayout.Button("Export Selected Nodes to JSON"))
            {
                if (selectedGraph == null)
                {
                    Debug.LogError("No graph selected.");
                    return;
                }
                ExportSelectedGraphToJSON(selectedGraph);
            }
        }

        private void EnsureRunner()
        {
            if (testRunner == null) testRunner = new XnoiseBasicUnitTests();
        }

        private void ExportSelectedGraphToJSON(NodeGraph graph)
        {
            var selectedNodes = Selection.objects.OfType<XNode.Node>().ToList();
            if (selectedNodes.Count == 0)
            {
                Debug.LogError("No nodes selected.");
                return;
            }

            var exportData = new GraphExport();

            foreach (var node in selectedNodes)
            {
                var nodeData = new NodeExport
                {
                    id = node.name,
                    guid = node.GetInstanceID(),
                    type = node.GetType().Name,
                    inputs = new Dictionary<string, object>()
                };

                foreach (FieldInfo field in node.GetType().GetFields())
                {
                    var attribs = field.GetCustomAttributes(typeof(InputAttribute), true);
                    if (attribs.Length > 0)
                    {
                        var inputPort = node.GetInputPort(field.Name);
                        if (inputPort != null && !inputPort.IsConnected)
                        {
                            nodeData.inputs[field.Name] = field.GetValue(node);
                        }
                    }
                }

                exportData.nodes.Add(nodeData);
            }

            // Collect connections
            foreach (var node in selectedNodes)
            {
                foreach (FieldInfo field in node.GetType().GetFields())
                {
                    var attribs = field.GetCustomAttributes(typeof(InputAttribute), true);
                    if (attribs.Length > 0)
                    {
                        var inputPort = node.GetInputPort(field.Name);
                        if (inputPort != null && inputPort.IsConnected)
                        {
                            var connection = new NodeConnectionExport
                            {
                                fromNodeGuid = inputPort.Connection.node.GetInstanceID(),
                                toNodeGuid = node.GetInstanceID(),
                                inputField = field.Name
                            };
                            exportData.connections.Add(connection);
                        }
                    }
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(exportData, Newtonsoft.Json.Formatting.Indented);
            string path = EditorUtility.SaveFilePanel("Save Graph JSON", Application.dataPath, "XnoiseGraphTest.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                Debug.Log("Graph exported to: " + path);
            }
        }
    }
}