using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagAttackForward : IActionAbility, IAttackAbility
{
    public int distance;
}

public class TagAttackForwardProvider : ProviderEcs<TagAttackForward> { }
