using UnityEngine;
using UnityEngine.UI;

namespace XNoise_DemoWebglPlayer
{
    public class ViewSelectionHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _graphDisplayer;
        [SerializeField] private Image _graphVisualizer;
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
            _graphDisplayer.SetActive(false);
            UIManager.RenderSceneStateChanged += UpdateSceneRender;
            UIManager.SelectedGradiantIndexChanged += UIManager_SelectedGradiantIndexChanged;
        }

        private void OnDestroy()
        {
            UIManager.RenderSceneStateChanged -= UpdateSceneRender;
            UIManager.SelectedGradiantIndexChanged -= UIManager_SelectedGradiantIndexChanged;
        }

        private void UpdateSceneRender(bool isRendering3D)
        {
            _graphDisplayer.SetActive(!isRendering3D);
            _cam.gameObject.SetActive(isRendering3D);
        }

        private void UIManager_SelectedGradiantIndexChanged(int obj)
        {
            enabled = true;
        }

        private void LateUpdate()
        {
            enabled = false;
            _graphVisualizer.sprite = GraphLibrary.CurrentGraphImage;
        }
    }
}