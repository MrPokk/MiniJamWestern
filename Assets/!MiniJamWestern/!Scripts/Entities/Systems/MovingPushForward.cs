using BitterECS.Core;
using UnityEngine;

public class PushForwardHandler
{
    public static void Execute(EcsEntity entity, GridComponent gridCom, ListActionComponent tag, TargetTo target)
    {
        var isType = tag.Is<TagPushForward>();
        if (!isType)
        {
            return;
        }

        if (!MovementUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction))
        {
            return;
        }

        var nextPos = gridCom.currentPosition + direction;

        if (GridInteractionHandler.TryGetEntityAt(nextPos, out var pushedEntity))
        {
            var pushTarget = nextPos + direction;
            if (GridInteractionHandler.IsPlacing(pushTarget))
            {
                GridInteractionHandler.MoveEntity(pushedEntity, pushTarget);
            }
            else
            {
                return;
            }
        }

        if (!GridInteractionHandler.IsPlacing(nextPos))
        {
            return;
        }

        GridInteractionHandler.MoveEntity(entity, nextPos);
        return;
    }
}

public class EnemyPushForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagEnemy, GridComponent, ListActionComponent, TargetTo> _filter;

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref TagEnemy tagEnemy, ref GridComponent gridCom, ref ListActionComponent tag, ref TargetTo target) =>
        {
            PushForwardHandler.Execute(e, gridCom, tag, target);
        });
    }
}
