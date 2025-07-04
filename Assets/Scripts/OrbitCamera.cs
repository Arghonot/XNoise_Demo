using UnityEngine;
using UnityEngine.EventSystems;

namespace XNoise_DemoWebglPlayer
{
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _minDistance = 2f;
        [SerializeField] private float _maxDistance = 15f;
        [SerializeField] private float _zoomSpeed = 2f;

        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private float _smoothTime = 0.1f;

        private  Vector2 _currentRotation;
        private  Vector2 _targetRotation;
        private  Vector2 _rotationVelocity;

        private float _originalDistance;
        private Vector3 _originalPosition;
        private Quaternion _originalQuaternion;

        void Start()
        {
            _originalPosition = transform.position;
            _originalQuaternion = transform.rotation;
            _originalDistance = _distance;

            Vector3 angles = transform.eulerAngles;
            _currentRotation = _targetRotation = new Vector2(angles.y, angles.x);

            ShortcutHandler.OnPressedR += ResetView;
            PreviewSceneManager.OnSelectPlane += ResetView;
        }

        private void OnDestroy()
        {
            ShortcutHandler.OnPressedR -= ResetView;
            PreviewSceneManager.OnSelectPlane -= ResetView;
        }

        [ContextMenu("ResetView")]
        private void ResetView()
        {
            _currentRotation = Vector2.zero;
            _targetRotation = Vector2.zero;
            //transform.position = _originalPosition;
            //transform.rotation = _originalQuaternion;
            _distance = _originalDistance;
            transform.LookAt(_target);
        }

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // if hovering UI
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

                _targetRotation.x += mouseX * _rotationSpeed;
                _targetRotation.y -= mouseY * _rotationSpeed;
                _targetRotation.y = Mathf.Clamp(_targetRotation.y, -80f, 80f);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            _distance -= scroll * _zoomSpeed;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }

        void SmoothRotation()
        {
            _currentRotation.x = Mathf.SmoothDamp(_currentRotation.x, _targetRotation.x, ref _rotationVelocity.x, _smoothTime);
            _currentRotation.y = Mathf.SmoothDamp(_currentRotation.y, _targetRotation.y, ref _rotationVelocity.y, _smoothTime);
        }

        void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(_currentRotation.y, _currentRotation.x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -_distance);
            Vector3 position = rotation * negDistance + _target.position;

            transform.position = position;
            transform.LookAt(_target);
        }
    }
}