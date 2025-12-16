using System;
using UnityEngine;

public class CameraAudioListener : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera targetCamera;

    private Vector3 position = Vector3.zero;
    
    private void LateUpdate()
    {
        position.z = -targetCamera.orthographicSize;
        transform.localPosition = position;
    }
}
