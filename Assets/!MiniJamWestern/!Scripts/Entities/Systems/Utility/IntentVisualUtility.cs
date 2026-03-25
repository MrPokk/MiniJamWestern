using System;
using System.Collections.Generic;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public static class IntentVisualUtility
{
    public static void DrawAbilityArea(EcsEntity owner, EcsEntity ability, Vector2Int origin, Vector2Int target, Color color, int salt = 0)
    {
        if (!owner.Has<IntentVisualTracker>())
            owner.Add(new IntentVisualTracker { activeRectIds = new List<int>() });

        ref var tracker = ref owner.Get<IntentVisualTracker>();
        ClearVisuals(owner);

        var dir = GetAttackDirection(owner, origin, target);
        var abilityCurrent = ability.Get<TagActions>().ability;
        if (abilityCurrent is TagAttackForward tagAttackForward)
        {
            for (var i = 1; i <= tagAttackForward.value; i++)
            {
                DrawTile(owner, ref tracker, origin + (dir * i), color, salt);
            }
        }
        else if (abilityCurrent is TagAttackTwoSides twoSides)
        {
            for (var i = 1; i <= twoSides.value; i++)
            {
                DrawTile(owner, ref tracker, origin + (dir * i), color, salt);
                DrawTile(owner, ref tracker, origin - (dir * i), color, salt);
            }
        }
        else if (abilityCurrent is TagRotation)
        {
            return;
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
        var rectId = HashCode.Combine(owner.GetHashCode(), gridPos.GetHashCode(), salt);

        DrawRectUtility.Instance?.DrawStaticRect(rectId, worldPos, 32f, color);

        Color fillColor = color;
        fillColor.a = 0.3f;
        DrawRectUtility.Instance?.DrawStaticFullRect(rectId, worldPos, 32f, fillColor);

        tracker.activeRectIds.Add(rectId);
    }

    public static Vector2Int GetAttackDirection(EcsEntity actor, Vector2Int actorPos, Vector2Int targetPos)
    {
        if (actor.TryGet<FacingComponent>(out var facing) && facing.direction != Vector2Int.zero)
            return facing.direction;

        var diff = targetPos - actorPos;
        if (diff == Vector2Int.zero) return Vector2Int.up;

        return new Vector2Int(Math.Sign(diff.x), Math.Sign(diff.y));
    }
}
