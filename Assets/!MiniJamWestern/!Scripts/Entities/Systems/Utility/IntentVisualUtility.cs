using System;
using System.Collections.Generic;
using BitterECS.Core;
using UnityEngine;

public static class IntentVisualUtility
{
    public static void DrawAbilityArea(EcsEntity owner, EcsEntity ability, Vector2Int origin, Vector2Int target, Color color, int salt = 0)
    {
        if (!owner.Has<IntentVisualTracker>())
            owner.Add(new IntentVisualTracker { activeRectIds = new List<int>() });

        ref var tracker = ref owner.Get<IntentVisualTracker>();
        ClearVisuals(owner);

        Vector2Int dir = GetAttackDirection(owner, origin, target);

        if (ability.TryGet<TagAttackForward>(out var forward))
        {
            for (int i = 1; i <= forward.distance; i++)
                DrawTile(owner, ref tracker, origin + (dir * i), color, salt);
        }
        else if (ability.TryGet<TagAttackTwoSides>(out var twoSides))
        {
            var diff = target - origin;
            if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) <= twoSides.distance)
            {
                DrawTile(owner, ref tracker, origin + dir, color, salt);
                DrawTile(owner, ref tracker, origin - dir, color, salt);
            }
        }
        else
        {
            DrawTile(owner, ref tracker, target, color, salt);
        }
    }

    public static void ClearVisuals(EcsEntity entity)
    {
        if (!entity.TryGet<IntentVisualTracker>(out var tracker)) return;
        if (tracker.activeRectIds == null) return;

        foreach (var id in tracker.activeRectIds)
        {
            DrawRectUtility.Instance?.HideStaticRect(id);
            DrawRectUtility.Instance?.HideStaticFullRect(id);
        }
        tracker.activeRectIds.Clear();
    }

    private static void DrawTile(EcsEntity owner, ref IntentVisualTracker tracker, Vector2Int gridPos, Color color, int salt)
    {
        if (!GridInteractionHandler.Instance.Playfield.IsWithinGrid(gridPos)) return;

        var worldPos = GridInteractionHandler.Instance.Playfield.ConvertingPosition(gridPos);
        int rectId = HashCode.Combine(owner.GetHashCode(), gridPos.GetHashCode(), salt);

        DrawRectUtility.Instance?.DrawStaticRect(rectId, worldPos, 32f, color);

        Color fillColor = color;
        fillColor.a = 0.3f;
        DrawRectUtility.Instance?.DrawStaticFullRect(rectId, worldPos, 32f, fillColor);

        tracker.activeRectIds.Add(rectId);
    }

    public static Vector2Int GetAttackDirection(EcsEntity actor, Vector2Int actorPos, Vector2Int targetPos)
    {
        if (actor.TryGet<FacingComponent>(out var facing))
            return facing.direction;

        var diff = targetPos - actorPos;
        return diff == Vector2Int.zero ? Vector2Int.up : new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
    }
}
