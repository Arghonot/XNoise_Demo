using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class PreviewSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objects;

        private void Awake()
        {
            UIManager.SelectedSceneObjectChanged += OnObjectSelectionChanged;
            OnObjectSelectionChanged(1);
        }

        private void OnObjectSelectionChanged(int arg0)
        {
            for (int i = 0; i < _objects.Length; i++)
            {
                _objects[i].SetActive(false);
            }

            _objects[arg0].SetActive(true);
        }

        private void OnDestroy()
        {
            UIManager.SelectedSceneObjectChanged -= OnObjectSelectionChanged;
        }
    }
}