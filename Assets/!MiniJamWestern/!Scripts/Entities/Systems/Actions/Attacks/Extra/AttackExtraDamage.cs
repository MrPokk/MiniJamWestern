using BitterECS.Core;
using UnityEngine;

public class AttackExtraDamage : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnAttack);

    private static void OnAttack(EcsEntity entity)
    {
        if (!entity.Has<TagAttackExtraDamage>()) return;

        var extraDamage = entity.Get<TagAttackExtraDamage>().value;
        ref var targetEntity = ref entity.Get<IsAttackerTo>().targetEntity;

        if (!targetEntity.IsAlive || !targetEntity.Has<HealthComponent>()) return;

        ref var health = ref targetEntity.Get<HealthComponent>();
        var currentHealth = health.GetCurrentHealth();
        var newHealth = currentHealth - extraDamage;

        health.SetHealth(newHealth);
        targetEntity.AddFrame<IsDamagedEvent>();

        Debug.Log($"[ExtraDamage] Attacker {entity.Id} applied +{extraDamage} bonus. Target {targetEntity.Id} HP: {newHealth}");

        if (newHealth <= 0 && !targetEntity.Has<IsDeadEvent>())
        {
            targetEntity.AddFrame<IsDeadEvent>();
            Debug.Log($"[ExtraDamage] Target {targetEntity.Id} killed by bonus damage.");
        }
    }
}
