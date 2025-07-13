using CustomGraph;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class GraphArgumentsHandler : MonoBehaviour
    {
        public static event Action<Selectable> OnFinishedLoadingArguments;
        public static GraphVariables Variables;

        [SerializeField] private VariableRowPool _pooler;
        [SerializeField] private Transform _rowsHolder;

        private void Start()
        {
            GraphLibrary.OnSelectedGraphChanged += LoadNewArguments;
            Variables = GraphLibrary.CurrentEditedGraphStorage.CreateDeepCopy();
            RefreshArguments();
        }

        private void OnDestroy() => GraphLibrary.OnSelectedGraphChanged -= LoadNewArguments;

        private void LoadNewArguments(CustomGraph.GraphVariables obj)
        {
            Variables = obj.CreateDeepCopy();

            UnloadPreviousArguments();
            RefreshArguments();
        }

        private void RefreshArguments()
        {
            foreach (var item in Variables)
            {
                if (item.Name == "Seed") continue;
                GraphVariableFieldUI row = GetCorrespondingRow(item);

                if (row != null)
                {
                    row.Setup(item.Name, item.GUID, item.GetValue());
                }
            }

            SetupAllRowsNavigation();
            if (_rowsHolder.childCount > 0) OnFinishedLoadingArguments?.Invoke(_rowsHolder.GetChild(0).GetComponentInChildren<Selectable>(false));
        }

        private void SetupAllRowsNavigation()
        {
            // Cache all active child GameObjects
            List<GameObject> activeRows = new List<GameObject>();

            for (int i = 0; i < _rowsHolder.childCount; i++)
            {
                Transform child = _rowsHolder.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    activeRows.Add(child.gameObject);
                }
            }

            int activeCount = activeRows.Count;

            if (activeCount < 2) return;

            // Loop through and setup navigation for each pair
            for (int i = 0; i < activeCount; i++)
            {
                int nextIndex = (i + 1) % activeCount;
                SetupNavigation(activeRows[i], activeRows[nextIndex]);
            }
        }

        // todo replace getcomponent by a dictionnary use 
        private GraphVariableFieldUI GetCorrespondingRow(VariableStorageRoot item)
        {
            var storedType = Variables.GetContainedType(item.GUID);

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