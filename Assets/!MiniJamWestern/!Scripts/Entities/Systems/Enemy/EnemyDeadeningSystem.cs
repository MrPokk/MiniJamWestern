using System;
using System.Linq;
using DG.Tweening;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class EnemyDeadeningSystem : IEcsInitSystem
{
    public Priority Priority => Priority.High;
    private EcsEvent _ecsEvent;
    private EcsFilter<TagInventoryStorage> _ecsEntities;

    public void Init()
    {
        _ecsEvent.SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagEnemy>(), added: OnDead);
    }

    private void OnDead(EcsEntity entity)
    {
        IntentVisualUtility.ClearVisuals(entity);

        entity.Remove<IsIntentComponent>();
        entity.Remove<EnemyStateComponent>();

        try
        {
            var inventory = entity.GetProvider<AbilityInventoryProvider>();
            if (inventory != null)
            {
                MoveAllToStorage(inventory);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[EnemyDeadeningSystem] Ошибка переноса лута в хранилище: {ex}");
        }

        entity.AddFrame<IsPreDestroyDeadEvent>();

        if (entity.TryGet<UnityComponent<Transform>>(out var transformComp) && transformComp.value != null)
        {
            var transform = transformComp.value;
            transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                if (entity.IsAlive)
                {
                    IntentVisualUtility.ClearVisuals(entity); // Контрольный вызов
                    entity.Destroy();
                }
            }).Play();
        }
        else
        {
            IntentVisualUtility.ClearVisuals(entity);
            entity.Destroy();
        }
    }

    private void MoveAllToStorage(AbilityInventoryProvider enemyInventory)
    {
        if (_ecsEntities.Count == 0) return;
        if (enemyInventory.Value.listSlot == null) return;

        var itemsToMove = enemyInventory.Value.listSlot
            .Where(slot => slot != null && slot.Value.itemEntity.IsAlive)
            .Select(slot => slot.Value.itemEntity.GetProvider<AbilityViewProvider>())
            .Where(provider => provider != null)
            .ToList();

        enemyInventory.ExtractAll();

        var storageEntity = _ecsEntities.First();
        var storageProvider = storageEntity.GetProvider<AbilityInventoryProvider>();

        foreach (var item in itemsToMove)
        {
            storageProvider.AddFirstEmpty(item);
        }
    }
}
