using BitterECS.Core;
using UnityEngine;

public class AttackExtraDamage
{
    public static void Execute(EcsEntity _, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackExtraDamage>(out var attackAbility)) return;
        if (!GridInteractionHandler.TryGetEntityAt(target.position, out var targetEntity)) return;

        ApplyEffect(targetEntity, attackAbility.value);
    }

    public static void ApplyEffect(EcsEntity target, int damageValue)
    {
        if (!target.IsAlive || !target.Has<HealthComponent>()) return;

        ref var health = ref target.Get<HealthComponent>();
        var newHealth = health.GetCurrentHealth() - damageValue;

        health.SetHealth(newHealth);
        target.AddFrame<IsDamagedEvent>();

        if (newHealth <= 0 && !target.Has<IsDeadEvent>())
        {
            target.AddFrame<IsDeadEvent>();
        }
    }
}


public class AttackExtraDamageAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeWhereEntity<IsAttackerTo>(e => !e.Has<TagPlayer>(), added: OnAttack);

    private static void OnAttack(EcsEntity attacker)
    {
        if (!attacker.TryGet<ListActionComponent>(out var list)) return;
        if (!list.Is<TagAttackExtraDamage>(out var tag)) return;

        if (!attacker.TryGet<IsAttackerTo>(out var attackInfo)) return;

        AttackExtraDamage.ApplyEffect(attackInfo.targetEntity, tag.value);
    }
}
