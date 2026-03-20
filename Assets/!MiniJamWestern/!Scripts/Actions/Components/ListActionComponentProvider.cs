using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;

[Serializable]
public class ListActionComponent
{
    public List<TagActions> abilities = new();

    public void AddAbility(TagActions ability)
    {
        if (ability.ability != null && !abilities.Contains(ability))
        {
            abilities.Add(ability);
        }
    }

    public void RemoveAbility(TagActions ability)
    {
        if (ability.ability != null)
        {
            abilities.Remove(ability);
        }
    }

    public bool Is<T>() where T : IActionAbility
    {
        if (abilities == null) return false;

        foreach (var ability in abilities)
        {
            if (ability.ability is T) return true;
        }
        return false;
    }

    public bool Is<T>(out T result) where T : IActionAbility
    {
        result = default;
        if (abilities == null) return false;

        foreach (var ability in abilities)
        {
            if (ability.ability is T t)
            {
                result = t;
                return true;
            }
        }
        return false;
    }
}

public class ListActionComponentProvider : ProviderEcs<ListActionComponent> { }
