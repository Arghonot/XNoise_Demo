using CustomGraph;
using System;
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
            GameObject FirstRow = null;
            GameObject previousRow = null;
            StringBuilder builder = new StringBuilder();

            foreach (var item in currentStorage)
            {
                if (item.Name == "Seed") continue;
                GraphVariableFieldUI row = GetCorrespondingRow(currentStorage, item);

                if (row != null)
                {
                    SetupItem(row, item, ref firstSelectable, ref previousRow);
                    if (FirstRow == null) FirstRow = row.gameObject;
                }
            }

            if (previousRow != null && FirstRow != null) SetupNavigation(previousRow, FirstRow);
            if (firstSelectable != null) OnFinishedLoadingArguments?.Invoke(firstSelectable);
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

        private void SetupItem(GraphVariableFieldUI row, VariableStorageRoot item, ref Selectable firstSelectable, ref GameObject previousRow)
        {
            row.Setup(item.Name, item.GUID, item.GetValue());
            if (firstSelectable == null) firstSelectable = row.GetComponentInChildren<Selectable>();
            if (previousRow != null) SetupNavigation(previousRow, row.gameObject);
            previousRow = row.gameObject;
        }

        private void SetupNavigation(GameObject A, GameObject B)
        {
            var previousRowSelectable = _pooler.GetSelectable(A);
            var currentRowSelectable = _pooler.GetSelectable(B);
            var nav = new Navigation { mode = Navigation.Mode.Explicit };

            nav.selectOnDown = currentRowSelectable;
            previousRowSelectable.navigation = nav;
        }

        private void UnloadPreviousArguments() => _pooler.ReturnAllActive();
    }
}