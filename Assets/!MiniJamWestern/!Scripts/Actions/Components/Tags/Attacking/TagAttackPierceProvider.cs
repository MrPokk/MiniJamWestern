using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;


[Serializable]
public struct TagAttackTwoSides : IActionAbility, IAttackAbility
{
    public int distance;

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackingTwoSidesHandler.Execute(actor, grid, list, target);
    }
}
