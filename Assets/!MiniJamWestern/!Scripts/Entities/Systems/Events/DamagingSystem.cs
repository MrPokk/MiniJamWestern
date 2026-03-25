using System;
using BitterECS.Core;
using UnityEngine;

public class DamagingSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnTarget);

    private static void OnTarget(EcsEntity entity)
    {
        if (!entity.TryGet<DamageConstComponent>(out var damageComp))
        {
            return;
        }

        ref var attackerTo = ref entity.Get<IsAttackerTo>();
        ref var targetEntity = ref attackerTo.targetEntity;

        if (!targetEntity.IsAlive || !targetEntity.Has<HealthComponent>())
        {
            return;
        }

        ref var health = ref targetEntity.Get<HealthComponent>();

        var newHealth = health.GetCurrentHealth() - damageComp.damage;
        health.SetHealth(newHealth);

        targetEntity.AddFrame<IsDamagedEvent>();

        if (health.GetCurrentHealth() <= 0)
        {
            if (!targetEntity.Has<IsDeadEvent>())
            {
                targetEntity.AddFrame<IsDeadEvent>();
            }
        }
    }
}

public struct IsDamagedEvent
{
}
