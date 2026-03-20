using BitterECS.Core;
using UnityEngine;

public class MovingPushForwardHandler
{
    public static void Execute(EcsEntity entity, GridComponent gridCom, ListActionComponent tag, TargetTo target)
    {
        if (!tag.Is<TagPushForward>()) return;

        if (!VectorUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction)) return;

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

        if (!GridInteractionHandler.IsPlacing(nextPos)) return;

        GridInteractionHandler.MoveEntity(entity, nextPos);
    }
}
