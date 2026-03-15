using BitterECS.Core;
using UnityEngine;

public class MovingForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, TagMoveForward, TargetToMove> _filter;

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref GridComponent gridCom, ref TagMoveForward tagMove, ref TargetToMove target) =>
        {
            if (!MovementUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction))
            {
                return;
            }

            var nextPos = gridCom.currentPosition + direction;

            if (!GridInteractionHandler.IsPlacing(nextPos))
            {
                return;
            }

            GridInteractionHandler.Moving(gridCom.currentPosition, nextPos);
        });
    }
}
