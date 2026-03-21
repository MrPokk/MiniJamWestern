using UnityEngine;

public class ScaleWithCameraView : MonoBehaviour
{
    [SerializeField]
    private float _scaleFactor = 1.0f;

    private Camera _cachedCamera;

    private float _lastOrthoSize;
    private float _lastAspect;
    private float _lastScaleFactor;

    private void Start()
    {
        UpdateCameraReference();
        ScaleToCamera();
    }

    private void LateUpdate()
    {
        if (_cachedCamera == null)
        {
            UpdateCameraReference();
            if (_cachedCamera == null) return;
        }

        if (_cachedCamera.orthographicSize != _lastOrthoSize ||
            _cachedCamera.aspect != _lastAspect ||
            _scaleFactor != _lastScaleFactor)
        {
            ScaleToCamera();
        }
    }

    private void OnValidate()
    {
        ScaleToCamera();
    }

    private void UpdateCameraReference()
    {
        _cachedCamera = Camera.main;
    }

    private void ScaleToCamera()
    {
        if (_cachedCamera == null)
            UpdateCameraReference();

        if (_cachedCamera == null)
            return;

        _lastOrthoSize = _cachedCamera.orthographicSize;
        _lastAspect = _cachedCamera.aspect;
        _lastScaleFactor = _scaleFactor;

        float cameraHeight = 2f * _cachedCamera.orthographicSize;
        float cameraWidth = cameraHeight * _cachedCamera.aspect;

        transform.localScale = new Vector3(cameraWidth * _scaleFactor, cameraHeight * _scaleFactor, 1f);
    }
}
