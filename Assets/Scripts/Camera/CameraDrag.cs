using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Camera
{
    public class CameraDrag : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float dragSpeed = 0.01f;
        [SerializeField] private float momentumMultiplier = 20f;
        [SerializeField] [Range(0.8f, 1f)] private float decelerationRate = 0.86f;
        [SerializeField] private float minimumVelocity = 0.001f;
        
        [Header("References")]
        private CinemachineCamera cinemachineCamera;
        private Vector2 mouseDelta;
        
        private bool isDragging = false;
        private bool isDecelerating = false;
        private bool subscribeToBoot = false;

        private float velocitySmoothDamp;
        private Vector2 lastDirection;
        private Vector3 move;
        
        private LimitedQueue<float> recentVelocities = new(30);
        
        private void Start()
        {
            cinemachineCamera = GetComponent<CameraManager>().GetCinemachine;
            Assert.IsNotNull(cinemachineCamera, "No Cinemachine Camera found.");
            
            if (!subscribeToBoot)
                SubscribeInput();
        }

        private void OnEnable()
        {
            if (subscribeToBoot || !InputManager.instance)
                return;
            
            SubscribeInput();
        }

        private void SubscribeInput()
        {
            subscribeToBoot = true;
            
            InputManager.instance.onMiddleMouseButtonPressStarted += StartDragging;
            InputManager.instance.onMouseDelta += RetrieveMouseDelta;
            InputManager.instance.onMiddleMouseButtonPressCanceled += StopDragging;
        }

        private void OnDisable()
        {
            subscribeToBoot = false;
            
            InputManager.instance.onMiddleMouseButtonPressStarted -= StartDragging;
            InputManager.instance.onMouseDelta -= RetrieveMouseDelta;
            InputManager.instance.onMiddleMouseButtonPressCanceled -= StopDragging;
        }

        private void Update()
        {
            if(isDecelerating)
            {
                EndDragCamera();
            }
        }

        private void RetrieveMouseDelta(Vector2 _delta)
        {
            mouseDelta = _delta;
            
            if (isDragging)
            {
                DragCamera();
            }
        }

        #region Movement Press
        
        private void StartDragging()
        {
            isDragging = true;
            isDecelerating = false;
        }

        private void StopDragging()
        {
            velocitySmoothDamp = 0;
            foreach (float _velocityAtPoint in recentVelocities.ToArray())
            {
                velocitySmoothDamp += _velocityAtPoint;
            }
            velocitySmoothDamp /= recentVelocities.ToArray().Length;
            
            isDragging = false;
            isDecelerating = true;
        }

        private void DragCamera()
        {
            move = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * dragSpeed;
            cinemachineCamera.transform.Translate(move, Space.World);
            recentVelocities.Enqueue(move.magnitude);
            lastDirection = move.normalized;
        }
        
        #endregion

        #region Movement Release

        private void EndDragCamera()
        {
            velocitySmoothDamp *= Mathf.Pow(decelerationRate, Time.deltaTime * 60f);
            
            cinemachineCamera.transform.Translate(velocitySmoothDamp * lastDirection, Space.World);
            
            if (velocitySmoothDamp < minimumVelocity)
            {
                isDecelerating = false;
                velocitySmoothDamp = 0f;
            }
        }

        #endregion
    }
}