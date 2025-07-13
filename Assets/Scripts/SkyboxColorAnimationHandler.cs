using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class SkyboxColorAnimationHandler : MonoBehaviour
    {
        [Header("Sine wave speed control")]
        [Tooltip("How many times the sine wave completes per second. Higher = faster.")]
        public float speedScale = 1.0f;

        [Header("Exposure bounds")]
        public float minExposure = 0.5f;
        public float maxExposure = 1.5f;
        [SerializeField] private Material _skyboxMaterial;

        void Update()
        {
            if (_skyboxMaterial == null)
            {
                Debug.LogWarning("SkyboxColorAnimationHandler: Skybox material not assigned!");
                return;
            }

            float t = Mathf.Sin(Time.time * speedScale);
            float normalized = (t + 1f) * 0.5f;

            float exposure = Mathf.Lerp(minExposure, maxExposure, normalized);
            _skyboxMaterial.SetFloat("_Exposure", exposure);
        }
    }
}