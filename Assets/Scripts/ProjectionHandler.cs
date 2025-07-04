using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class ProjectionHandler : MonoBehaviour
    {
        public static int ProjectionType = 0;
        [SerializeField] private UIManager _uiManager;
        //[SerializeField] private int[] _projectionTypes;

        private void Awake() => UIManager.SelectedProjectionTypeIndexChanged += UpdateProjectionTypeIndex;

        private void OnDestroy() => UIManager.SelectedProjectionTypeIndexChanged -= UpdateProjectionTypeIndex;

        private void UpdateProjectionTypeIndex(int obj) => ProjectionType = obj;

        //public int GetCurrentGradient() => _projectionTypes[_uiManager.projectionType.value];
    }
}