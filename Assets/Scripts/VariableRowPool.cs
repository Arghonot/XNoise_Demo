using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public enum VariableRowType { Numerical, Boolean }

    public class VariableRowPool : MonoBehaviour
    {
        public static Transform RowContainer;
        [Header("Prefabs")]
        [SerializeField] private GameObject _floatVariableRowPrefab;
        [SerializeField] private GameObject _booleanVariableRowPrefab;

        [Header("Parent Container")]
        [SerializeField] private Transform _rowContainer;

        private class Pool
        {
            public GameObject prefab;
            public Transform parent;
            public List<GameObject> allInstances = new List<GameObject>();
            public Stack<GameObject> available = new Stack<GameObject>();
        }

        private Dictionary<VariableRowType, Pool> _pools;
        private Dictionary<GameObject, Selectable> _selectablePerRow;

        void Awake()
        {
            _selectablePerRow = new Dictionary<GameObject, Selectable>();
            _pools = new Dictionary<VariableRowType, Pool>()
            {
                { VariableRowType.Numerical, new Pool { prefab = _floatVariableRowPrefab, parent = _rowContainer } },
                { VariableRowType.Boolean, new Pool { prefab = _booleanVariableRowPrefab, parent = _rowContainer } }
            };

            RowContainer = _rowContainer;
        }

        public GameObject RequestRow(VariableRowType type)
        {
            var pool = _pools[type];
            GameObject row;

            if (pool.available.Count > 0)
            {
                row = pool.available.Pop();
            }
            else
            {
                row = Instantiate(pool.prefab, pool.parent);
                pool.allInstances.Add(row);
                _selectablePerRow.Add(row, row.GetComponentInChildren<Selectable>());
            }

            row.SetActive(true);
            return row;
        }

        public void ReturnRow(VariableRowType type, GameObject row)
        {
            row.SetActive(false);
            _pools[type].available.Push(row);
        }

        /// <summary>
        /// Return ALL rows, disabling them and adding to the pool.
        /// </summary>
        public void ReturnAllActive()
        {
            foreach (var pool in _pools.Values)
            {
                foreach (var row in pool.allInstances)
                {
                    if (row.activeSelf)
                    {
                        row.SetActive(false);
                        if (!pool.available.Contains(row))
                            pool.available.Push(row);
                    }
                }
            }
        }

        public Selectable GetSelectable(GameObject gameObject)
        {
            return _selectablePerRow[gameObject];
        }
    }
}