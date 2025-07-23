using CustomGraph;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class GraphArgumentsHandler : MonoBehaviour
    {
        public static event Action<Selectable> OnFinishedLoadingArguments;
        [SerializeField] private VariableRowPool _pooler;
        public static CustomGraph.GraphVariables currentStorage;

        private void Start()
        {
            GraphLibrary.OnSelectedGraphChanged += LoadNewArguments;
            LoadNewArguments(GraphLibrary.CurrentEditedGraphStorage);
        }
        private void OnDestroy() => GraphLibrary.OnSelectedGraphChanged -= LoadNewArguments;

        private void LoadNewArguments(CustomGraph.GraphVariables obj)
        {
            UnloadPreviousArguments();
            currentStorage = obj.CreateDeepCopy();
            RefreshArguments();
        }

        private void RefreshArguments()
        {
            Selectable firstSelectable = null;
            StringBuilder builder = new StringBuilder();

            foreach (var item in currentStorage)
            {
                if (item.Name == "Seed") continue;
                GraphVariableFieldUI row = GetCorrespondingRow(currentStorage, item);

                if (row != null)
                {
                    firstSelectable = _pooler.GetSelectable(row.gameObject);
                    row.Setup(item.Name, item.GUID, item.GetValue());
                }
            }
            BuildRowNavigation();
            if (firstSelectable != null) OnFinishedLoadingArguments?.Invoke(firstSelectable);
        }

        public void BuildRowNavigation()
        {
            Transform container = VariableRowPool.RowContainer;

            List<Selectable> activeRows = new List<Selectable>();

            for (int i = 0; i < container.childCount; i++)
            {
                GameObject rowGO = container.GetChild(i).gameObject;
                if (!rowGO.activeSelf) continue;
                Selectable sel = _pooler.GetSelectable(rowGO);
                if (sel != null) activeRows.Add(sel);
            }

            int count = activeRows.Count;
            if (count <= 1) return;

            for (int i = 0; i < count; i++)
            {
                Selectable current = activeRows[i];
                Selectable next = activeRows[(i + 1) % count];

                Navigation nav = current.navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnDown = next;
                nav.selectOnUp = null;
                nav.selectOnRight = null;
                nav.selectOnLeft = null;
                current.navigation = nav;
            }
        }

        private GraphVariableFieldUI GetCorrespondingRow(GraphVariables obj, VariableStorageRoot item)
        {
            var storedType = obj.GetContainedType(item.GUID);

            if (storedType == typeof(float) || storedType == typeof(double) || storedType == typeof(int))
            {
                return _pooler.RequestRow(VariableRowType.Numerical).GetComponent<GraphVariableFieldUI>();
            }
            else if (storedType == typeof(bool))
            {
                return _pooler.RequestRow(VariableRowType.Boolean).GetComponent<GraphVariableFieldUI>();
            }

            return null;
        }

        private void UnloadPreviousArguments() => _pooler.ReturnAllActive();
    }
}