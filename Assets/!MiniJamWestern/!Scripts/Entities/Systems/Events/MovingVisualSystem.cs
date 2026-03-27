using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using DG.Tweening;
using UnityEngine;

public class MovingVisualSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private const float MoveDuration = 0.3f;
    private const Ease MoveEase = Ease.OutQuad;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsMovingEvent>(OnMover);

    private static void OnMover(EcsEntity entity)
    {
        if (entity.TryGetProvider<ProviderEcs>(out var transformComp) &&
            entity.TryGet<GridComponent>(out var grid))
        {
            var transform = transformComp.gameObject.transform;
            transform.DOComplete();
            var baseRotation = transform.localRotation;

            var targetWorldPos = grid.gridPresenter.ConvertingPosition(grid.currentPosition);

            transform.DOKill();
            transform.DOMove(targetWorldPos, MoveDuration).SetEase(MoveEase).Play();
            transform.DOPunchRotation(new Vector3(0, 0, 12f), MoveDuration, 3, 0.5f).Play();
            transform.DOLocalRotateQuaternion(baseRotation, 0.1f)
                .SetDelay(MoveDuration - 0.05f)
                .Play();
        }
    }
}
