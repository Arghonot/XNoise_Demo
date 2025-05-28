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
using Xnoise;

namespace CustomUnitTesting
{
    public class XnoiseUnitTestWindow : EditorWindow
    {
        [MenuItem("Window/Xnoise/Run Unit Tests")]
        public static void ShowWindow()
        {
            GetWindow<XnoiseUnitTestWindow>("Xnoise Tests");
        }

        private NodeGraph selectedGraph;

        private void OnGUI()
        {
            GUILayout.Label("Xnoise Unit Test Utilities", EditorStyles.boldLabel);

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

            if (GUILayout.Button("Import JSON to New Graph"))
            {
                ImportGraphFromJSON();
            }
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
                    type = node.GetType().AssemblyQualifiedName,
                    position = node.position,
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
            exportData.blackboardPosition = ((XnoiseGraph)selectedNodes.First().graph).blackboard.position;

            string path = EditorUtility.SaveFilePanel("Save Graph JSON", Application.dataPath, "XnoiseGraphTest.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(exportData, settings);
                File.WriteAllText(path, json);
                Debug.Log("Graph exported to: " + path);
            }
        }

        private void ImportGraphFromJSON()
        {
            string path = EditorUtility.OpenFilePanel("Load Graph JSON", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            GraphExport data = Newtonsoft.Json.JsonConvert.DeserializeObject<GraphExport>(json);

            var graph = ScriptableObject.CreateInstance<XnoiseGraph>();

            string folderPath = EditorUtility.SaveFolderPanel("Select Folder To Save Graph", Application.dataPath, "");
            if (string.IsNullOrEmpty(folderPath)) return;

            string fileName = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(graph, "Assets" + folderPath.Replace(Application.dataPath, "") + "/" + fileName + ".asset");

            Dictionary<int, XNode.Node> createdNodes = new Dictionary<int, XNode.Node>();

            foreach (var nodeExport in data.nodes)
            {
                Type nodeType = ResolveType(nodeExport.type);
                if (nodeType == null)
                {
                    Debug.LogError("Could not find type: " + nodeExport.type);
                    continue;
                }

                var node = graph.AddNode(nodeType);
                node.name = nodeExport.id;
                node.position = nodeExport.position;
                createdNodes[nodeExport.guid] = node;

                foreach (var kvp in nodeExport.inputs)
                {
                    FieldInfo field = nodeType.GetField(kvp.Key);
                    if (field != null && field.GetCustomAttribute(typeof(InputAttribute)) != null)
                    {
                        try
                        {
                            object value = kvp.Value;
                            if (field.FieldType.IsEnum)
                            {
                                value = Enum.ToObject(field.FieldType, value);
                            }
                            else
                            {
                                value = Convert.ChangeType(value, field.FieldType);
                            }
                            field.SetValue(node, value);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Failed to set input value '{kvp.Key}' on node '{node.name}': {ex.Message}");
                        }
                    }
                }
            }
            if (((XnoiseGraph)graph).blackboard != null)
            {
                ((XnoiseGraph)graph).blackboard.position = data.blackboardPosition;
            }

            foreach (var conn in data.connections)
            {
                if (createdNodes.TryGetValue(conn.fromNodeGuid, out var fromNode) &&
                    createdNodes.TryGetValue(conn.toNodeGuid, out var toNode))
                {
                    NodePort fromPort = fromNode.GetOutputPort("Output");
                    NodePort toPort = toNode.GetInputPort(conn.inputField);
                    if (fromPort != null && toPort != null)
                    {
                        toPort.Connect(fromPort);
                    }
                }
            }

            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            Debug.Log("Graph successfully imported from JSON");
        }

        private Type ResolveType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null) return type;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(typeName);
                if (type != null) return type;
            }

            return null;
        }

        [Serializable]
        public class GraphExport
        {
            public List<NodeExport> nodes = new List<NodeExport>();
            public List<NodeConnectionExport> connections = new List<NodeConnectionExport>();
            public Vector2 blackboardPosition;
        }

        [Serializable]
        public class NodeExport
        {
            public string id;
            public int guid;
            public string type;
            public Vector2 position;
            public Dictionary<string, object> inputs;
        }

        [Serializable]
        public class NodeConnectionExport
        {
            public int fromNodeGuid;
            public int toNodeGuid;
            public string inputField;
        }
    }
}