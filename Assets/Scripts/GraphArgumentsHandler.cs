using CustomGraph;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;

namespace XNoise_DemoWebglPlayer
{


    public class GraphArgumentsHandler : MonoBehaviour
    {
        public static event Action<Selectable> OnFinishedLoadingArguments;

        [SerializeField] private VariableRowPool _pooler;

        private void Start()
        {
            GraphLibrary.OnSelectedGraphChanged += LoadNewArguments;
            RefreshArguments(GraphLibrary.CurrentEditedGraphStorage);
        }
        private void OnDestroy() => GraphLibrary.OnSelectedGraphChanged -= LoadNewArguments;

        private void LoadNewArguments(CustomGraph.GraphVariables obj)
        {
            UnloadPreviousArguments();
            RefreshArguments(obj);
        }

        private void RefreshArguments(CustomGraph.GraphVariables obj)
        {
            Selectable firstSelectable = null;
            GameObject FirstRow = null;
            GameObject previousRow = null;

            foreach (var item in obj)
            {
                if (item.Name == "Seed") continue;
                GraphVariableFieldUI row = GetCorrespondingRow(obj, item);

                if (row != null)
                {
                    SetupItem(row, item, ref firstSelectable, ref previousRow);
                    if (FirstRow == null) FirstRow = row.gameObject;
                    SetupItem(row, item, ref firstSelectable, ref previousRow);
                }
            }

            SetupNavigation(previousRow, FirstRow);

            OnFinishedLoadingArguments?.Invoke(firstSelectable);
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