using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagAttackPush : IActionAbility, IAttackAbility, IComponentPush
{
    [SerializeField] private int _distance;

    public int distance
    {
        get => _distance;
        set => _distance = value;
    }

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackingPushHandler.Execute(actor, grid, list, target);
    }
}

public interface IComponentPush
{
    public int distance { get; set; }
}
