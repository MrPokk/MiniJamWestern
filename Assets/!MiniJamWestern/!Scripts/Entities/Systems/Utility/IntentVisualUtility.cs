using System;
using System.Collections.Generic;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public static class IntentVisualUtility
{
    private const float TileSize = 32f;
    private const float FillAlpha = 0.3f;

    public static void DrawAbilityArea(EcsEntity owner, EcsEntity ability, Vector2Int origin, Vector2Int target, Color color, int salt = 0)
    {
        var tracker = GetOrAddTracker(owner);
        ClearVisuals(owner);

        var direction = GetAttackDirection(owner, origin, target);
        var abilityEffect = ability.Get<TagActions>().ability;

        switch (abilityEffect)
        {
            case TagAttackForward forward:
                DrawLine(owner, tracker, origin, direction, forward.value, color, salt);
                break;

            case TagAttackTwoSides twoSides:
                DrawLine(owner, tracker, origin, direction, twoSides.value, color, salt);
                DrawLine(owner, tracker, origin, -direction, twoSides.value, color, salt);
                break;

            case TagRotation:
                break;

            default:
                DrawTile(owner, tracker, target, color, salt);
                break;
        }
    }

    public static void ClearVisuals(EcsEntity entity)
    {
        if (!entity.TryGet<IntentVisualTracker>(out var tracker) || tracker.activeRectIds == null)
            return;

        var drawer = DrawRectUtility.Instance;
        if (drawer == null) return;

        foreach (var id in tracker.activeRectIds)
        {
            drawer.HideStaticRect(id);
            drawer.HideStaticFullRect(id);
        }

        tracker.activeRectIds.Clear();
    }

    private static void DrawLine(EcsEntity owner, IntentVisualTracker tracker, Vector2Int origin, Vector2Int direction, int length, Color color, int salt)
    {
        for (var i = 1; i <= length; i++)
        {
            DrawTile(owner, tracker, origin + (direction * i), color, salt);
        }
    }

    private static void DrawTile(EcsEntity owner, IntentVisualTracker tracker, Vector2Int gridPos, Color color, int salt)
    {
        var playfield = GridInteractionHandler.Instance.Playfield;
        if (!playfield.IsWithinGrid(gridPos)) return;

        if (owner.Has<TagEnemy>() && GridInteractionHandler.TryGetEntityAt(gridPos, out var entityAtTile))
        {
            if (entityAtTile.Has<TagEnemy>()) return;
        }

        var drawer = DrawRectUtility.Instance;
        if (drawer == null) return;

        var worldPos = playfield.ConvertingPosition(gridPos);
        var rectId = HashCode.Combine(owner.GetHashCode(), gridPos.GetHashCode(), salt);

        drawer.DrawStaticRect(rectId, worldPos, TileSize, color);

        var fillColor = color;
        fillColor.a = FillAlpha;
        drawer.DrawStaticFullRect(rectId, worldPos, TileSize, fillColor);

        tracker.activeRectIds.Add(rectId);
    }

    public static Vector2Int GetAttackDirection(EcsEntity actor, Vector2Int actorPos, Vector2Int targetPos)
    {
        if (actor.TryGet<FacingComponent>(out var facing) && facing.direction != Vector2Int.zero)
            return facing.direction;

        var diff = targetPos - actorPos;
        if (diff == Vector2Int.zero)
            return Vector2Int.up;

        return new Vector2Int(Math.Sign(diff.x), Math.Sign(diff.y));
    }

    private static IntentVisualTracker GetOrAddTracker(EcsEntity entity)
    {
        if (!entity.Has<IntentVisualTracker>())
        {
            entity.Add(new IntentVisualTracker { activeRectIds = new List<int>() });
        }
        return entity.Get<IntentVisualTracker>();
    }
}
