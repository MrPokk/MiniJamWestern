using System;
using BitterECS.Integration.Unity;

[Flags]
public enum AbilityTypeLimit
{
    None = 0,
    Attack = 1 << 0,
    Move = 1 << 1,
    Rotation = 1 << 2,
    All = Attack | Move | Rotation
}

[Serializable]
public struct AbilitySlotLimitComponent
{
    public AbilityTypeLimit allowedTypes;

    public bool IsAllowed(IActionAbility ability)
    {
        if (allowedTypes == AbilityTypeLimit.All) return true;
        if (ability == null) return false;

        if (ability is IAttackAbility && (allowedTypes & AbilityTypeLimit.Attack) != 0) return true;
        if (ability is IMoveAbility && (allowedTypes & AbilityTypeLimit.Move) != 0) return true;
        if (ability is IRotationAbility && (allowedTypes & AbilityTypeLimit.Rotation) != 0) return true;

        return false;
    }

    public bool IsAllowed(ListActionComponent listAction)
    {
        if (listAction == null || listAction.abilities.Count == 0) return false;

        foreach (var action in listAction.abilities)
        {
            if (IsAllowed(action.ability))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAllowed(TagActions tagAction)
    {
        return IsAllowed(tagAction.ability);
    }
}

public class AbilitySlotLimitProvider : ProviderEcs<AbilitySlotLimitComponent>
{
}
