using System;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class PreviewSceneManager : MonoBehaviour
    {
        public static event Action OnSelectPlane;
        [SerializeField] private GameObject[] _objects;

        private void Awake()
        {
            UIManager.SelectedSceneObjectChanged += OnObjectSelectionChanged;
            OnObjectSelectionChanged(0);
        }

        private void OnDestroy()
        {
            UIManager.SelectedSceneObjectChanged -= OnObjectSelectionChanged;
        }

        private void OnObjectSelectionChanged(int arg0)
        {
            for (int i = 0; i < _objects.Length; i++)
            {
                _objects[i].SetActive(false);
            }

            if (arg0 == 0) OnSelectPlane?.Invoke();
            _objects[arg0].SetActive(true);
        }
    }
}