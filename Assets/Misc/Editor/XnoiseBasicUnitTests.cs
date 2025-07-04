using System;
using UnityEditor;
using UnityEngine;

namespace CustomUnitTesting
{
    public static class XnoiseBasicUnitTests
    {
        public static void GetActiveType()
        {
            var selectedNode = Selection.activeObject as XNode.Node;
            string typeName = GetNodeTypeName(selectedNode);
            Debug.Log("Node type: " + typeName);
        }

        public static void RetrieveTypeByName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) Debug.Log($"<color=red>[ResolveType] Could not find type: {typeName}</color>");

            Type resolvedType = Type.GetType(typeName);
            if (resolvedType != null) Debug.Log($"<color=green>[ResolveType] type: {typeName} was findeable</color>");

            // Fallback: search all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                resolvedType = assembly.GetType(typeName);
                if (resolvedType != null)
                {
                    Debug.Log($"<color=green>[ResolveType] type: {typeName} was findeable</color>");
                    return;
                }
            }

            Debug.Log($"<color=red>[ResolveType] Could not find type: {typeName}</color>");
        }

        //public void TestGPUvsCPU()
        //{
        //    Debug.Log("[UnitTest] TestGPUvsCPU launched.");
        //}

        public static string GetNodeTypeName(XNode.Node node)
        {
            if (node == null) return null;
            return node.GetType().AssemblyQualifiedName;
        }
    }
}