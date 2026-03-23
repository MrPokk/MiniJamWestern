using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraObjectComponent : MonoBehaviour
{
    [SerializeField] private CameraShakingComponent _cameraShakingComponent;

    public Camera CameraTarget { get; private set; }
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulseSource;

    private void Awake()
    {
        CameraTarget = GetComponentInChildren<Camera>();
        _cinemachineImpulseSource ??= GetComponent<CinemachineImpulseSource>();
        _cinemachineCamera ??= GetComponentInChildren<CinemachineCamera>();
    }

    public void ShakeCamera() => ShakeCamera(_cameraShakingComponent.shakeAmount, _cameraShakingComponent.shakeDuration);
    public void ShakeCamera(float shakeAmount, float shakeDuration)
    {
        _cinemachineImpulseSource.ImpulseDefinition.ImpulseDuration = shakeDuration;
        _cinemachineImpulseSource.GenerateImpulse(shakeAmount);
    }
}


[Serializable]
public struct CameraShakingComponent
{
    public float shakeDuration;
    public float shakeAmount;
}
