using BitterECS.Core;
using UnityEngine;

public class MovingForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, IsActionComponent, TargetToMove> _filter = new();

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref GridComponent gridCom, ref IsActionComponent tag, ref TargetToMove target) =>
        {
            var isType = tag.Is<TagMoveForward>();
            if (!isType)
            {
                return;
            }

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
