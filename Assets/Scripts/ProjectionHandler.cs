using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class ProjectionHandler : MonoBehaviour
    {
        public static int ProjectionType = 0;

        private void Awake() => UIManager.SelectedProjectionTypeIndexChanged += UpdateProjectionTypeIndex;
        private void OnDestroy() => UIManager.SelectedProjectionTypeIndexChanged -= UpdateProjectionTypeIndex;
        private void UpdateProjectionTypeIndex(int obj) => ProjectionType = obj;
    }
}