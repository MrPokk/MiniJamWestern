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

        var distance = Mathf.Abs(target.position.x - grid.currentPosition.x) + Mathf.Abs(target.position.y - grid.currentPosition.y);
        var maxSteps = Mathf.Min(attackAbility.value, distance);

        for (var i = 1; i <= maxSteps; i++)
        {
            var attackPos = grid.currentPosition + (dir * i);
            if (GridInteractionHandler.TryGetEntityAt(attackPos, out var entityTo))
            {
                entity.AddFrame<IsAttackerTo>(new(entityTo));
                break;
            }
        }
    }
}
