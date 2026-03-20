using BitterECS.Core;
using UnityEngine;

public class AttackRegenerationSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnAttack);

    private static void OnAttack(EcsEntity entity)
    {
        if (!entity.TryGet<TagAttackRegeneration>(out var attackRegeneration) || !entity.Has<HealthComponent>())
            return;

        ref var health = ref entity.Get<HealthComponent>();
        var newHealth = health.GetCurrentHealth() + attackRegeneration.value;

        health.SetHealth(newHealth);

        Debug.Log($"[Regeneration] Entity {entity.Id} healed +{attackRegeneration.value} HP. Current HP: {newHealth}");
    }
}
