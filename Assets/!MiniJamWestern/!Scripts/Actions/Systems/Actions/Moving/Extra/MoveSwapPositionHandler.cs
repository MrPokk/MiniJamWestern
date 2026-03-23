using BitterECS.Core;
using UnityEngine;

public class SwapPositionHandler
{
    public static void Execute(EcsEntity actor, GridComponent actorGrid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagMoveSwapPosition>()) return;

        if (!GridInteractionHandler.TryGetEntityAt(target.position, out var targetEntity)) return;

        var actorOldPos = actorGrid.currentPosition;
        var targetOldPos = target.position;

        var targetProvider = GridInteractionHandler.Extraction(targetOldPos);
        if (targetProvider == null) return;

        if (GridInteractionHandler.MoveEntity(actor, targetOldPos))
        {
            GridInteractionHandler.Placing(actorOldPos, targetProvider);

            actor.AddFrame<IsMovingEvent>();
            targetEntity.AddFrame<IsMovingEvent>();

            Debug.Log($"[Swap] {actor.Id} swapped with {targetEntity.Id}");
        }
        else
        {
            GridInteractionHandler.Placing(targetOldPos, targetProvider);
        }
    }
}


public class SwapPositionAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeWhereEntity<IsAttackerTo>(e => !e.Has<TagPlayer>(), added: OnAttack);

    private static void OnAttack(EcsEntity attacker)
    {
        if (!attacker.TryGet<GridComponent>(out var grid)) return;
        if (!attacker.TryGet<ListActionComponent>(out var list)) return;
        if (!list.Is<TagMoveSwapPosition>()) return;

        if (!attacker.TryGet<IsAttackerTo>(out var attackInfo)) return;
        var targetEntity = attackInfo.targetEntity;

        if (!targetEntity.IsAlive || !targetEntity.Has<GridComponent>()) return;

        var targetData = new TargetTo { position = targetEntity.Get<GridComponent>().currentPosition };
        SwapPositionHandler.Execute(attacker, grid, list, targetData);
    }
}
