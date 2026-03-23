using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using DG.Tweening;
using UnityEngine;

public class RotationVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Low;
    private EcsEvent _genericEvent;

    public void Init()
    {
        _genericEvent.Subscribe<IsRotationEvent>(added: OnEffect);
    }

    private void OnEffect(EcsEntity entity)
    {
        if (!entity.TryGet<UnityComponent<Transform>>(out var view)) return;

        var transform = view.value;

        ref var state = ref entity.GetOrAdd<RotationStateComponent>();
        var targetY = state.isRotated ? 0f : 180f;
        state.isRotated = !state.isRotated;

        transform.DORotate(new Vector3(0, targetY, 0), 0.25f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad)
            .Play();
    }
}

public struct RotationStateComponent
{
    public bool isRotated;
}
