using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;

public class TagAttackRegenerationProvider : ProviderEcs<TagAttackRegeneration>
{

}

[Serializable]
public struct TagAttackRegeneration : IActionAbility, IAttackAbility
{
    public int value;

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackRegenerationHandler.Execute(actor, grid, list, target);
    }
}
