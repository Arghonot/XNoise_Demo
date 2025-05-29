using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xnoise;

namespace CustomUnitTesting
{
    public static class XNoiseJsonUnitTester
    {
        private static readonly string _graphLocalPath = "Graphs";
        private static readonly string _imgLocalPath = "Imgs";

        public static void RunTestsFromFolder(string sourceDir, string targetDir)
        {
            string[] jsonFiles = Directory.GetFiles(sourceDir, "*.json", SearchOption.AllDirectories);

            string imgOut = Path.Combine(targetDir, _imgLocalPath);
            string graphOut = Path.Combine(targetDir, _graphLocalPath);
            string logFile = Path.Combine(targetDir, $"logs_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            Directory.CreateDirectory(imgOut);
            Directory.CreateDirectory(graphOut);

            List<string> logs = new List<string>();
            int successCount = 0, failureCount = 0;

            foreach (string jsonPath in jsonFiles)
            {
                string relativePath = Path.GetRelativePath(sourceDir, jsonPath);
                string flatName = Path.ChangeExtension(relativePath, null).Replace(Path.DirectorySeparatorChar, '_');
                string displayName = Path.Combine(relativePath.Split(Path.DirectorySeparatorChar)).Replace("\\", " ").Replace("/", " ");

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    XnoiseGraph graph = XNoiseGraphSelectionSaverLoader.ImportGraphFromJSON(jsonPath, graphOut);

                    foreach (bool isGPU in new[] { true, false })
                    {
                        Texture2D result = graph.Render(isGPU);
                        if (result != null)
                        {
                            byte[] bytes = result.EncodeToPNG();
                            string variantPath = Path.Combine(imgOut, flatName + (isGPU ? "_GPU.png" : "_CPU.png"));
                            File.WriteAllBytes(variantPath, bytes);
                        }
                    }

                    sw.Stop();
                    logs.Add($"Testing {displayName} : OK ({sw.ElapsedMilliseconds}ms)");
                    successCount++;
                }
                catch (Exception e)
                {
                    sw.Stop();
                    logs.Add($"Testing {displayName} : Failed ({sw.ElapsedMilliseconds}ms) {e.GetType().Name}\n{e}");
                    failureCount++;
                }
            }

            logs.Add("");
            logs.Add($"Success : {successCount} files.");
            logs.Add($"Failure : {failureCount} files.");

            File.WriteAllLines(logFile, logs);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log($"Unit testing complete. Logs written to: {logFile}");
        }

        private static void LoadGraph()
        {

        }

        private static void TestGraph()
        {

        }
    }
}