using System;
using BitterECS.Core;
using UnityEngine;

public class MoneyAddPlayerSystem : IEcsInitSystem
{
    public Priority Priority => Priority.FIRST_TASK;

    private EcsEvent _ecsEvent;
    private EcsFilter<TagPlayerMoney, MoneyComponent> _ecsEntities;

    public void Init()
    {
        _ecsEvent.SubscribeWhereEntity<IsPreDestroyDeadEvent>(e => e.Has<TagEnemy>(), added: OnDead);
    }

    private void OnDead(EcsEntity entity)
    {
        if (!entity.TryGet<MoneyComponent>(out var moneyComponent))
        {
            Debug.Log("MoneyComponent not found for entity: " + entity);
            return;
        }

        foreach (var entity1 in _ecsEntities)
        {
            ref var playerMoney = ref entity1.Get<MoneyComponent>();
            playerMoney.SetMoney(playerMoney.GetCurrentMoney() + moneyComponent.GetCurrentMoney());
            entity1.AddFrame<PlayerUpdateMoneyUIEvent>();

            Debug.Log($"Added money to player: {playerMoney.GetCurrentMoney()}");
        }
    }
}
