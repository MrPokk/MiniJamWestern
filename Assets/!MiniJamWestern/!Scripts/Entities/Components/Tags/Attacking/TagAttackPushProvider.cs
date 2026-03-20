using System;
using BitterECS.Integration.Unity;

public class TagAttackPushProvider : ProviderEcs<TagAttackPush>
{ }

[Serializable]
public struct TagAttackPush : IActionAbility, IAttackAbility
{
    public int distance;
}
