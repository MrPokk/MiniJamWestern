using System;
using BitterECS.Core;
using UnityEngine;

public class DamageSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnTarget);

    private static void OnTarget(EcsEntity entity)
    {
        if (!entity.Has<DamageConstComponent>())
            return;

        var damageComp = entity.Get<DamageConstComponent>();

        ref var targetEntity = ref entity.Get<IsAttackerTo>().targetEntity;

        if (!targetEntity.IsAlive || !targetEntity.Has<HealthComponent>())
            return;

        ref var health = ref targetEntity.Get<HealthComponent>();
        var deltaHealth = health.GetCurrentHealth() - damageComp.damage;
        health.SetHealth(deltaHealth);

        Debug.Log(health.GetCurrentHealth());

        return;
    }

}
