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
        if (!entity.Has<DamageConstComponent>())
        {
            return;
        }

        var damageComp = entity.Get<DamageConstComponent>();
        ref var attackerTo = ref entity.Get<IsAttackerTo>();
        ref var targetEntity = ref attackerTo.targetEntity;

        if (!targetEntity.IsAlive)
        {
            return;
        }

        if (!targetEntity.Has<HealthComponent>())
        {
            return;
        }

        ref var health = ref targetEntity.Get<HealthComponent>();

        var oldHealth = health.GetCurrentHealth();
        var damageTaken = damageComp.damage;
        var deltaHealth = oldHealth - damageTaken;

        health.SetHealth(deltaHealth);
        targetEntity.AddFrame<IsDamagedEvent>();

        if (deltaHealth <= 0 && !targetEntity.Has<IsDeadEvent>())
        {
            targetEntity.AddFrame<IsDeadEvent>();
        }

        return;
    }
}

public struct IsDamagedEvent
{
}
