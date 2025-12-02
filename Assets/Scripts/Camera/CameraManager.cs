using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Assertions;

namespace Camera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Settings")]
        
        [Header("Components")]
        private CinemachineCamera cinemachineCamera;
        
        private void Awake()
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
            Assert.IsNotNull(cinemachineCamera, "Cinemachine Camera component is missing on CameraManager GameObject");
        }
        
        public CinemachineCamera GetCinemachine => cinemachineCamera;
    }
}