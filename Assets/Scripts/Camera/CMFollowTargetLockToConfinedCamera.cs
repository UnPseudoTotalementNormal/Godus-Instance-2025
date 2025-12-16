using Unity.Cinemachine;
using UnityEngine;

public class CMFollowTargetLockToConfinedCamera : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineCamera cmCam;
    [SerializeField] private Transform followTarget;
    [SerializeField] private CinemachineConfiner2D confiner2D;

    float _lastOrthoSize;

    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        if (cmCam != null) _lastOrthoSize = cmCam.Lens.OrthographicSize;
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void Update()
    {
        if (confiner2D != null && cmCam != null)
        {
            float s = cmCam.Lens.OrthographicSize;
            if (!Mathf.Approximately(s, _lastOrthoSize))
            {
                _lastOrthoSize = s;
                confiner2D.InvalidateLensCache();
            }
        }
    }

    void OnCameraUpdated(CinemachineBrain b)
    {
        if (b != brain || followTarget == null || cmCam == null)
            return;

        if (brain.ActiveVirtualCamera != (ICinemachineCamera)cmCam)
            return;

        Vector3 camPos = brain.OutputCamera.transform.position;
        followTarget.position = new Vector3(camPos.x, camPos.y, followTarget.position.z);
    }
}