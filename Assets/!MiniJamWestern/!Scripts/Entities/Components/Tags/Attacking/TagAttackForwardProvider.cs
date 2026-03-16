using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagAttackForward : IActionAbility
{
    public int distance;
}

public class TagAttackForwardProvider : ProviderEcs<TagAttackForward> { }
