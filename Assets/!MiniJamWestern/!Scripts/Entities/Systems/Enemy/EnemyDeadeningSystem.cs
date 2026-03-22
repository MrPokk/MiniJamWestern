using System;
using System.Linq;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class EnemyDeadeningSystem : IEcsInitSystem
{
    public Priority Priority => Priority.High;
    private EcsEvent _ecsEvent;

    private EcsFilter<TagInventoryStorage> _ecsEntities;
    private EcsFilter<TagEnemy> _ecsEntitiesEnemy;

    public void Init()
    {
        _ecsEvent.SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagEnemy>(), added: OnDead);
    }

    private void OnDead(EcsEntity entity)
    {
        if (entity.TryGet<GridComponent>(out var gridCom))
        {
            GridInteractionHandler.Extraction(gridCom.currentPosition);
        }

        var inventory = entity.GetProvider<AbilityInventoryProvider>();
        if (inventory != null)
        {
            MoveAllToStorage(inventory);
        }

        entity.Remove<IsIntentComponent>();

        if (!entity.Has<IsPreDestroyDeadEvent>())
        {
            entity.AddFrame<IsPreDestroyDeadEvent>();
        }
        entity.Destroy();
    }

    private void MoveAllToStorage(AbilityInventoryProvider enemyInventory)
    {
        if (_ecsEntities.Count == 0)
        {
            Debug.LogWarning("[EnemyDeadeningSystem] Хранилище не найдено! Лут уничтожен вместе с врагом.");
            return;
        }

        if (enemyInventory.Value.listSlot == null) return;

        var itemsToMove = enemyInventory.Value.listSlot
            .Where(slot => slot != null && slot.Value.itemEntity.IsAlive)
            .Select(slot => slot.Value.itemEntity.GetProvider<AbilityViewProvider>())
            .Where(provider => provider != null) // Защита от NullReference
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
