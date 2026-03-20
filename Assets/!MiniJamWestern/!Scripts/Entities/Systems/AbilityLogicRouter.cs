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
        switch (ability)
        {
            case TagMoveForward:
                MovingForwardHandler.Execute(actor, grid, list, target);
                break;
            case TagAttackForward:
                AttackingForwardHandler.Execute(actor, grid, list, target);
                break;
            case TagPushForward:
                PushForwardHandler.Execute(actor, grid, list, target);
                break;
        }
    }
}
