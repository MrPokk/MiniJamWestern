using BitterECS.Core;
using UnityEngine;

public static class RotationHandler
{
    public static void Execute(EcsEntity entity, GridComponent grid, ref TargetTo target)
    {
        if (!VectorUtility.TryGetStepDirection(grid.currentPosition, target.position, out var newDir))
        {
            newDir = entity.Has<FacingComponent>() ? entity.Get<FacingComponent>().direction : Vector2Int.up;
        }

        ref var facing = ref entity.GetOrAdd<FacingComponent>();

        if (facing.direction == newDir)
        {
            newDir = -newDir;
        }

        facing.direction = newDir;
        target.position = grid.currentPosition + newDir;
    }
}
