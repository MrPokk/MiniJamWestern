using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagActions
{
    [SerializeReference, SelectImplementation(typeof(IActionAbility))]
    public IActionAbility ability;
}

public interface IActionAbility { }

public class TagActionsProvider : ProviderEcs<TagActions> { }
