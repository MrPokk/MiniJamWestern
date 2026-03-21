using BitterECS.Core;
using UnityEngine;

public static class PushUtility
{
    public static void PushEntity(GridComponent grid, TargetTo target, IComponentPush attackAbility)
    {
        var diff = target.position - grid.currentPosition;
        var distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);

        if (distance <= attackAbility.distance)
        {
            if (GridInteractionHandler.TryGetEntityAt(target.position, out var entityTo))
            {
                var pushDir = new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
                if (pushDir != Vector2Int.zero)
                {
                    var newPos = target.position + pushDir;

                    if (GridInteractionHandler.IsPlacing(newPos))
                    {
                        GridInteractionHandler.MoveEntity(entityTo, newPos);
                    }
                }
            }
        }
    }
}

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
