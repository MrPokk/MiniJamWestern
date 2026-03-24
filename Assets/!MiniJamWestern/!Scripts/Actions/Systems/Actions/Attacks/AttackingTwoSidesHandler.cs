using BitterECS.Core;
using UnityEngine;

public class AttackingTwoSidesHandler
{
    public static void Execute(EcsEntity entity, GridComponent grid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackTwoSides>(out var ability)) return;

        var diff = target.position - grid.currentPosition;
        var distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);

        if (distance > ability.value) return;

        var dir = new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
        if (dir == Vector2Int.zero) return;

        var forwardPos = grid.currentPosition + dir;
        var backwardPos = grid.currentPosition - dir;

        if (GridInteractionHandler.TryGetEntityAt(forwardPos, out var targetForward))
        {
            entity.AddFrame<IsAttackerTo>(new(targetForward));
        }

        if (GridInteractionHandler.TryGetEntityAt(backwardPos, out var targetBackward))
        {
            entity.AddFrame<IsAttackerTo>(new(targetBackward));
        }
    }
}
