using System;
using BitterECS.Core;

[Serializable]
public struct TagMoveSwapPosition : IActionAbility, IMoveAbility
{
    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        SwapPositionHandler.Execute(actor, grid, list, target);
    }
}
