using Graph;
using static XNode.Node;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using XNode;
using Xnoise;
using Newtonsoft.Json;

namespace CustomUnitTesting
{
    public static class XNoiseGraphSelectionSaverLoader
    {
        public static void ExportSelectedGraphToJSON(NodeGraph graph)
        {
            var selectedNodes = UnityEditor.Selection.objects.OfType<XNode.Node>().ToList();
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

            exportData.blackboardPosition = ((XnoiseGraph)selectedNodes.First().graph).blackboard.position;

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

            string path = UnityEditor.EditorUtility.SaveFilePanel("Save Graph JSON", Application.dataPath, "XnoiseGraphTest.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string json = JsonConvert.SerializeObject(exportData, settings);
                File.WriteAllText(path, json);
                Debug.Log("Graph exported to: " + path);
            }
        }

        public static XnoiseGraph ImportGraphFromJSON(string openPath, string savePath)
        {
            if (string.IsNullOrEmpty(openPath)) return null;

            string json = File.ReadAllText(openPath);
            GraphExport data = JsonConvert.DeserializeObject<GraphExport>(json);

            var graph = ScriptableObject.CreateInstance<XnoiseGraph>();


            if (string.IsNullOrEmpty(savePath)) return null;

            string fileName = Path.GetFileNameWithoutExtension(openPath);
            UnityEditor.AssetDatabase.CreateAsset(graph, "Assets" + savePath.Replace(Application.dataPath, "") + "/" + fileName + ".asset");

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

            UnityEditor.EditorUtility.SetDirty(graph);
            UnityEditor.AssetDatabase.SaveAssets();
            graph.Initialize();
            Debug.Log("Graph successfully imported from JSON");
            return graph;
        }

        private static Type ResolveType(string typeName)
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
            public Vector2? blackboardPosition;
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