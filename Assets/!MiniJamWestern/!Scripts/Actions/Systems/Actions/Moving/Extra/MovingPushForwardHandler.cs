using BitterECS.Core;
using UnityEngine;

public class MovingPushForwardHandler
{
    public static void Execute(EcsEntity actor, GridComponent gridCom, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagMovePushForward>(out var movePush)) return;

        if (GridInteractionHandler.TryGetEntityAt(target.position, out var victim))
        {
            if (victim.Id != actor.Id)
            {

                AttackExtraDamage.ApplyEffect(victim, movePush.value);

                Debug.Log($"[Push] {actor.Id} толкает и наносит урон {victim.Id}");
            }
        }

        PushUtility.PushEntity(gridCom, target, movePush);

        if (VectorUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var dir))
        {
            var nextPos = gridCom.currentPosition + dir;
            if (GridInteractionHandler.IsPlacing(nextPos))
            {
                GridInteractionHandler.MoveEntity(actor, nextPos);
            }
        }

        AttackingForwardHandler.Execute(actor, gridCom, list, target);
    }
}

public class MovingPushForwardAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeWhereEntity<IsAttackerTo>(e => !e.Has<TagPlayer>(), added: OnAttack);

    private static void OnAttack(EcsEntity attacker)
    {
        if (!attacker.TryGet<GridComponent>(out var grid)) return;
        if (!attacker.TryGet<ListActionComponent>(out var list)) return;

        if (!list.Is<TagMovePushForward>(out _)) return;

        if (!attacker.TryGet<IsAttackerTo>(out var attackInfo)) return;

        var targetEntity = attackInfo.targetEntity;
        if (!targetEntity.IsAlive || !targetEntity.Has<GridComponent>()) return;

        var targetData = new TargetTo { position = targetEntity.Get<GridComponent>().currentPosition };

        MovingPushForwardHandler.Execute(attacker, grid, list, targetData);
    }
}
