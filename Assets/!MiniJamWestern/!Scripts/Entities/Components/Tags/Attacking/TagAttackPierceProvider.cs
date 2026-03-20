using System;
using BitterECS.Integration.Unity;

public class TagAttackTwoSidesProvider : ProviderEcs<TagAttackTwoSides>
{ }

[Serializable]
public struct TagAttackTwoSides : IActionAbility, IAttackAbility
{
    public int distance;
}
