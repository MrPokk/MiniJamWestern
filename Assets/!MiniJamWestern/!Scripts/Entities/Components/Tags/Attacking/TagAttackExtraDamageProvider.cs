using BitterECS.Integration.Unity;

public class TagAttackExtraDamageProvider : ProviderEcs<TagAttackExtraDamage>
{ }

public struct TagAttackExtraDamage : IActionAbility, IAttackAbility
{
    public int value;
}
