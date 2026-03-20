using BitterECS.Core;

public class MovingForwardHandler
{
    public static void Execute(EcsEntity entity, GridComponent gridCom, ListActionComponent tag, TargetTo target)
    {
        if (!tag.Is<TagMoveForward>()) return;

        if (!VectorUtility.TryGetStepDirection(gridCom.currentPosition, target.position, out var direction)) return;

        entity.GetOrAdd<FacingComponent>().direction = direction;

        var nextPos = gridCom.currentPosition + direction;
        if (!GridInteractionHandler.IsPlacing(nextPos)) return;

        GridInteractionHandler.Moving(gridCom.currentPosition, nextPos);
    }
}
