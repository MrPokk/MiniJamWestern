using BitterECS.Core;

public class AttackingPushHandler
{
    public static void Execute(EcsEntity _, GridComponent grid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackPush>(out var attackAbility)) return;

        PushUtility.PushEntity(grid, target, attackAbility);
    }
}

public class AttackingPushHandlerAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeWhereEntity<IsAttackerTo>(e => !e.Has<TagPlayer>(), added: OnAttack);

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

        AttackingPushHandler.Execute(attacker, grid, list, targetData);
    }
}
