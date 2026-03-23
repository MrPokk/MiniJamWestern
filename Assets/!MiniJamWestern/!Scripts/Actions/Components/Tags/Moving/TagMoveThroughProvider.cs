using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagMoveThrough : IActionAbility, IMoveAbility
{
    [SerializeField] private int _passDistance;

    public int distance
    {
        get => _passDistance;
        set => _passDistance = value;
    }

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        MovingThroughHandler.Execute(actor, grid, list, ref target);
    }
}
