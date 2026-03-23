using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using DG.Tweening;
using UnityEngine;

public class AttackVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Low;

    private EcsEvent _genericEvent;

    private CameraObjectComponent _cameraObject;

    public void Setup(CameraObjectComponent cameraObject)
    {
        _cameraObject = cameraObject;
    }

    public void Init()
    {
        _genericEvent.Subscribe<IsAttackerTo>(added: OnEffect);
    }

    private void OnEffect(EcsEntity attacker)
    {
        ShakeEntity(attacker);
        ShakeCamera();

    }

    private void ShakeCamera()
    {
        _cameraObject.ShakeCamera();
    }

    private static void ShakeEntity(EcsEntity attacker)
    {
        if (!attacker.TryGet<IsAttackerTo>(out var info)) return;
        if (!attacker.TryGet<UnityComponent<Transform>>(out var view)) return;
        if (!attacker.TryGet<GridComponent>(out var aGrid)) return;
        if (!info.targetEntity.TryGet<GridComponent>(out var tGrid)) return;

        var transform = view.value.transform;
        var diff = tGrid.currentPosition - aGrid.currentPosition;
        var direction = new Vector3(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1), 0).normalized;

        transform.DOPunchPosition(direction * 0.7f, 0.25f, 1, 0.1f).Play();
    }
}
