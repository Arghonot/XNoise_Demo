using UnityEngine;

namespace XNoise_DemoWebglPlayer
{
    public class OrbitCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5f;
        public float minDistance = 2f;
        public float maxDistance = 15f;
        public float zoomSpeed = 2f;

        public float rotationSpeed = 5f;
        public float smoothTime = 0.1f;

        private Vector2 currentRotation;
        private Vector2 targetRotation;
        private Vector2 rotationVelocity;

        void Start()
        {
            if (target == null)
                target = new GameObject("CameraTarget").transform;

            Vector3 angles = transform.eulerAngles;
            currentRotation = targetRotation = new Vector2(angles.y, angles.x);
        }

        void Update()
        {
            HandleInput();
            SmoothRotation();
            UpdateCameraPosition();
        }

        void HandleInput()
        {
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                targetRotation.x += mouseX * rotationSpeed;
                targetRotation.y -= mouseY * rotationSpeed;
                targetRotation.y = Mathf.Clamp(targetRotation.y, -80f, 80f);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        void SmoothRotation()
        {
            currentRotation.x = Mathf.SmoothDamp(currentRotation.x, targetRotation.x, ref rotationVelocity.x, smoothTime);
            currentRotation.y = Mathf.SmoothDamp(currentRotation.y, targetRotation.y, ref rotationVelocity.y, smoothTime);
        }

        void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.position = position;
            transform.LookAt(target);
        }
    }
}