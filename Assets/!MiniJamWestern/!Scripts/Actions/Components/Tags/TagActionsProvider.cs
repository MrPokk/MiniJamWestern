using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagActions
{
    [SerializeReference, SelectImplementation(typeof(IActionAbility))]
    public IActionAbility ability;
}

public interface IActionAbility
{
    void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target);
}

public class TagActionsProvider : ProviderEcs<TagActions> { }
