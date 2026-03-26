using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadeningSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private static bool s_isProcessingDeath = false;

    private EcsEvent _ecsEvent = new EcsEvent()
    .SubscribeWhereEntity<IsDeadEvent>(e => EcsConditions.Has<TagPlayer, GridComponent>(e), added: OnPlayerDead);

    private static void OnPlayerDead(EcsEntity entity)
    {
        if (s_isProcessingDeath) return;

        s_isProcessingDeath = true;

        OnDeadCoroutine(entity).Forget();
    }

    private static async UniTask OnDeadCoroutine(EcsEntity entity)
    {
        try
        {
            if (entity.TryGet<UnityComponent<Transform>>(out var transformComp) && transformComp.value != null)
            {
                var transform = transformComp.value;
                transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    entity.Destroy();
                }).Play();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            UIController.OpenScreen<UIToDefeatFloating>();
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
            UIController.CloseScreen();
        }
        finally
        {
            s_isProcessingDeath = false;
        }
    }
}
