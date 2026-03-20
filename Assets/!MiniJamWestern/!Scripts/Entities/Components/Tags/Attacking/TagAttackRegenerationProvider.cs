using System;
using BitterECS.Integration.Unity;

public class TagAttackRegenerationProvider : ProviderEcs<TagAttackRegeneration>
{

}

[Serializable]
public struct TagAttackRegeneration : IActionAbility, IAttackAbility
{
    public int value;
}
