using TMPro;
using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class ProjectionHandler : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private int[] _projectionTypes;

        // implement positions here if needed
        public int GetCurrentGradient() => _projectionTypes[_uiManager.projectionType.value];
    }
}