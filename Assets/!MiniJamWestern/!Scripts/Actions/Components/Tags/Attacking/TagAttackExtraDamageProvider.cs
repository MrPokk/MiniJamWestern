using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagAttackExtraDamage : IActionAbility, IAttackAbility
{
    public int value;

    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        AttackExtraDamage.Execute(actor, list, target);
    }
}
