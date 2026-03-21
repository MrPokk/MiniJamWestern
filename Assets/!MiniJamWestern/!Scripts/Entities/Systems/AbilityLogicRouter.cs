using BitterECS.Core;

public static class AbilityLogicRouter
{
    public static void Execute(EcsEntity actor, IActionAbility ability, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        if (ability == null) return;
        list ??= new();

        ability.Execute(actor, ref grid, list, ref target);
    }
}
