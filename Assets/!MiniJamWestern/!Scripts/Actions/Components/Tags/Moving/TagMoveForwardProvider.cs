using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagMoveForward : IActionAbility, IMoveAbility
{
    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        MovingForwardHandler.Execute(actor, grid, list, target);
    }
}
