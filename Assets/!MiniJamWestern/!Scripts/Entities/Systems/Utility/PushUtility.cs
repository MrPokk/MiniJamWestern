using UnityEngine;

public static class PushUtility
{
    public static void PushEntity(GridComponent grid, TargetTo target, IComponentPush attackAbility)
    {
        var diff = target.position - grid.currentPosition;
        var distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);

        if (distance <= attackAbility.distance)
        {
            if (GridInteractionHandler.TryGetEntityAt(target.position, out var entityTo))
            {
                var pushDir = new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
                if (pushDir != Vector2Int.zero)
                {
                    var newPos = target.position + pushDir;

                    if (GridInteractionHandler.IsPlacing(newPos))
                    {
                        GridInteractionHandler.MoveEntity(entityTo, newPos);
                    }
                }
            }
        }
    }
}
