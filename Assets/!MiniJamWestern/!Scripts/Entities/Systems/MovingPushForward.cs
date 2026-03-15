using BitterECS.Core;
using UnityEngine;

public class PushForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, TagPushForward, TargetToMove> _filter;

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref GridComponent gridCom, ref TagPushForward tag, ref TargetToMove target) =>
        {
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

            GridInteractionHandler.MoveEntity(e, nextPos);
        });
    }
}
