using BitterECS.Core;
using UnityEngine;

public class AttackingForwardHandler
{
    public static void Execute(EcsEntity entity, GridComponent grid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackForward>(out var attackAbility)) return;

        Vector2Int dir;
        if (entity.TryGet<FacingComponent>(out var facing))
            dir = facing.direction;
        else
            VectorUtility.TryGetStepDirection(grid.currentPosition, target.position, out dir);

        for (var i = 1; i <= attackAbility.distance; i++)
        {
            var attackPos = grid.currentPosition + (dir * i);
            if (GridInteractionHandler.TryGetEntityAt(attackPos, out var entityTo))
            {
                entity.AddFrame<IsAttackerTo>(new(entityTo));
            }
        }
    }
}
