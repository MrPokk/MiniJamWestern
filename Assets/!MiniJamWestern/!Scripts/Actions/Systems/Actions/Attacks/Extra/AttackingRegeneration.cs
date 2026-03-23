using BitterECS.Core;
using UnityEngine;

public class AttackRegenerationHandler
{
    public static void Execute(EcsEntity actor, GridComponent _, ListActionComponent list, TargetTo __)
    {
        if (list == null || !list.Is<TagAttackRegeneration>(out var tag)) return;

        ApplyEffect(actor, tag.value);
    }

    public static void ApplyEffect(EcsEntity entity, int healValue)
    {
        if (!entity.IsAlive || !entity.Has<HealthComponent>()) return;

        ref var health = ref entity.Get<HealthComponent>();
        var newHealth = health.GetCurrentHealth() + healValue;

        health.SetHealth(newHealth);

        Debug.Log($"[Regeneration] Entity {entity.Id} healed +{healValue} HP. Current HP: {newHealth}");
    }
}

public class AttackRegenerationAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnAttack);

    private static void OnAttack(EcsEntity attacker)
    {
        if (!attacker.TryGet<GridComponent>(out var grid)) return;
        if (!attacker.TryGet<ListActionComponent>(out var list)) return;
        if (!attacker.TryGet<IsAttackerTo>(out var attackInfo)) return;

        var targetEntity = attackInfo.targetEntity;
        if (!targetEntity.IsAlive || !targetEntity.Has<GridComponent>()) return;

        var targetData = new TargetTo
        {
            position = targetEntity.Get<GridComponent>().currentPosition
        };

        AttackRegenerationHandler.Execute(attacker, grid, list, targetData);
    }
}
