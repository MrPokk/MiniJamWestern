using BitterECS.Core;

public class MovingForwardHandler
{
    public static void Execute(EcsEntity _, GridComponent gridCom, ListActionComponent tag, TargetTo target)
    {
        if (!tag.Is<TagMoveForward>())
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
    }
}

public class EnemyMovingForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagEnemy, GridComponent, ListActionComponent, TargetTo> _filter;

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref TagEnemy tagEnemy, ref GridComponent gridCom, ref ListActionComponent tag, ref TargetTo target) =>
        {
            MovingForwardHandler.Execute(e, gridCom, tag, target);
        });
    }
}
