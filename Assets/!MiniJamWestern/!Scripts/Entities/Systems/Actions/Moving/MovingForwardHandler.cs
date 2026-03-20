using BitterECS.Core;
using UnityEngine;

public class MovingForwardHandler
{
    public static void Execute(EcsEntity _, GridComponent gridCom, ListActionComponent tag, TargetTo target)
    {
        if (!tag.Is<TagMoveForward>()) return;

        if (!MovementUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction)) return;

        var nextPos = gridCom.currentPosition + direction;

        if (!GridInteractionHandler.IsPlacing(nextPos)) return;

        GridInteractionHandler.Moving(gridCom.currentPosition, nextPos);
    }
}
