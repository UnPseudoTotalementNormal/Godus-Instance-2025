using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Assertions;

namespace Camera
{
    [RequireComponent(typeof(CameraManager))]
    public class CameraZoom : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float zoomSmoothTime = 0.2f;
        [Space(10)]
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 20f;
        
        [Header("References")]
        private CameraManager cameraManager;
        private CinemachineCamera cinemachineCamera;
        
        private float targetOrthographicSize;
        private float zoomVelocity;

        private void Start()
        {
            cameraManager = GetComponent<CameraManager>();
            cinemachineCamera = cameraManager.GetCinemachine;
            Assert.IsNotNull(cinemachineCamera, "No Cinemachine Camera found.");

            targetOrthographicSize = cinemachineCamera.Lens.OrthographicSize;
        }

        private void OnEnable()
        {
            InputManager.instance.onMiddleMousseScroll += Zoom;
        }
        
        private void OnDisable()
        {
            InputManager.instance.onMiddleMousseScroll -= Zoom;
        }

        private void Update()
        {
            UpdateZoom();
        }
        
        
        public void Zoom(float _zoomDelta)
        {
            //Debug.Log("Zoom Delta: " + _zoomDelta);
            targetOrthographicSize -= _zoomDelta * zoomSpeed;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);
        }

        public void SetZoomLimits(float _min, float _max)
        {
            minZoom = _min;
            maxZoom = _max;
            
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minZoom, maxZoom);
        }

        private void UpdateZoom()
        {
            float _currentSize = cinemachineCamera.Lens.OrthographicSize;
            float _newSize = Mathf.SmoothDamp(_currentSize, targetOrthographicSize, ref zoomVelocity, zoomSmoothTime);
            
            LensSettings _lens = cinemachineCamera.Lens;
            _lens.OrthographicSize = _newSize;
            cinemachineCamera.Lens = _lens;
        }

        #region Public Properties
        
        public float ZoomSpeed
        {
            get => zoomSpeed;
            set => zoomSpeed = value;
        }

        public float ZoomSmoothTime
        {
            get => zoomSmoothTime;
            set => zoomSmoothTime = value;
        }

        public float MinZoom
        {
            get => minZoom;
            set => SetZoomLimits(value, maxZoom);
        }

        public float MaxZoom
        {
            get => maxZoom;
            set => SetZoomLimits(minZoom, value);
        }

        public float CurrentZoom => cinemachineCamera  ? cinemachineCamera.Lens.OrthographicSize : 0f;
        
        #endregion
    }
}

