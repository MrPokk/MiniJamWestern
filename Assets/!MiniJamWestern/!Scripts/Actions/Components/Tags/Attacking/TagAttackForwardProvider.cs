using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagAttackForward : IActionAbility, IAttackAbility, IComponentValue
{
    [SerializeField] private int _value;

    public int value
    {
        get => _value;
        set => _value = value;
    }

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackingForwardHandler.Execute(actor, grid, list, target);
    }
}

