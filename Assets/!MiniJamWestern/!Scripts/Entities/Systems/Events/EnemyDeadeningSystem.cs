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

    public void Init()
    {
        _ecsEvent.SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagEnemy>(), added: OnDead);
    }

    private void OnDead(EcsEntity entity)
    {
        var inventory = entity.GetProvider<AbilityInventoryProvider>();
        if (inventory != null)
        {
            MoveAllToStorage(inventory);
        }

        entity.Destroy();
    }

    private void MoveAllToStorage(AbilityInventoryProvider enemyInventory)
    {

        var itemsToMove = enemyInventory.Value.listSlot
            .Where(slot => slot.Value.itemEntity.IsAlive)
            .Select(slot => slot.Value.itemEntity.GetProvider<AbilityViewProvider>())
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
