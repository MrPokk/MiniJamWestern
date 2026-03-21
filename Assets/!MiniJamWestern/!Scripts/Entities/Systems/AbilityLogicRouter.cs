using BitterECS.Core;

public static class AbilityLogicRouter
{
    public static void Execute(
        EcsEntity actor,
        IActionAbility ability,
        ref GridComponent grid,
        ListActionComponent list,
        ref TargetTo target)
    {

        list ??= new();

        switch (ability)
        {
            case TagRotation:
                RotationHandler.Execute(actor, grid, ref target);
                break;
            case TagMoveForward:
                MovingForwardHandler.Execute(actor, grid, list, target);
                break;
            case TagPushForward:
                MovingPushForwardHandler.Execute(actor, grid, list, target);
                break;
            case TagAttackForward:
                AttackingForwardHandler.Execute(actor, grid, list, target);
                break;
            case TagAttackTwoSides:
                AttackingTwoSidesHandler.Execute(actor, grid, list, target);
                break;
            case TagAttackPush:
                AttackingPushHandler.Execute(actor, grid, list, target);
                break;
        }
    }
}
