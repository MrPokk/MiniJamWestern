using BitterECS.Core;

public static class AbilityLogicRouter
{
    public static void Execute(EcsEntity actor, IActionAbility ability)
    {
        if (!actor.TryGet<GridComponent>(out var grid) ||
            !actor.TryGet<ListActionComponent>(out var list) ||
            !actor.TryGet<TargetTo>(out var target)) return;

        if (ability is TagMoveForward)
        {
            MovingForwardHandler.Execute(actor, grid, list, target);
        }
        else if (ability is TagAttackForward)
        {
            AttackingForwardHandler.Execute(actor, grid, list, target);
        }
        else if (ability is TagPushForward)
        {
            PushForwardHandler.Execute(actor, grid, list, target);
        }
    }
}
