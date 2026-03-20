using UnityEngine;

public static class VectorUtility
{
    public static bool TryGetStepDirection(Vector2Int currentPos, Vector2Int targetPos, out Vector2Int direction)
    {
        direction = Vector2Int.zero;

        if (currentPos == targetPos)
        {
            return false;
        }

        var diff = targetPos - currentPos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            direction.x = (int)Mathf.Sign(diff.x);
        }
        else
        {
            direction.y = (int)Mathf.Sign(diff.y);
        }

        return true;
    }
}
