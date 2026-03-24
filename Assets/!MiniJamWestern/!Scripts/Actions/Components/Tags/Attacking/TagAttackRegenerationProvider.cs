using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class TagAttackRegenerationProvider : ProviderEcs<TagAttackRegeneration>
{

}

[Serializable]
public struct TagAttackRegeneration : IActionAbility, IAttackAbility, IComponentValue
{
    [SerializeField] private int _value;

    public int value
    {
        get => _value;
        set => _value = value;
    }

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackRegenerationHandler.Execute(actor, grid, list, target);
    }
}
