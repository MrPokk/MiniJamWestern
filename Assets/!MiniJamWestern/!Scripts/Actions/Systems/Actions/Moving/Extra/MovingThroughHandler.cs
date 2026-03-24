using BitterECS.Core;
using UnityEngine;

public class MovingThroughHandler
{
    public static void Execute(EcsEntity actor, GridComponent gridCom, ListActionComponent list, ref TargetTo target)
    {
        if (!list.Is<TagMoveThrough>(out var ability)) return;

        if (!VectorUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction)) return;

        var currentPos = gridCom.currentPosition;

        var nextPos = currentPos + direction;
        var landingPos = currentPos + (direction * ability.value);

        if (GridInteractionHandler.TryGetEntityAt(nextPos, out var entityOnWay))
        {
            if (GridInteractionHandler.IsPlacing(landingPos))
            {
                GridInteractionHandler.MoveEntity(actor, landingPos);
                Debug.Log($"[Phase] {actor.Id} passed through {entityOnWay.Id} to {landingPos}");
            }
        }
        else
        {
            if (GridInteractionHandler.IsPlacing(nextPos))
            {
                GridInteractionHandler.MoveEntity(actor, nextPos);
            }
        }
    }
}

public class MovingThroughEnemyAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeWhereEntity<IsMovingEvent>(e => !e.Has<TagPlayer>(), added: OnIntent);

    private static void OnIntent(EcsEntity entity)
    {
        if (!entity.TryGet<GridComponent>(out var grid)) return;
        if (!entity.TryGet<ListActionComponent>(out var list)) return;

        if (!entity.TryGet<IsIntentComponent>(out var intent)) return;
        if (intent.chosenAbility is not TagMoveThrough) return;

        var targetData = new TargetTo { position = intent.targetPosition };
        MovingThroughHandler.Execute(entity, grid, list, ref targetData);

        entity.Remove<IsIntentComponent>();
    }
}
