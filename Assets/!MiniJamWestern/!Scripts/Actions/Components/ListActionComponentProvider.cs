using System;
using System.Collections.Generic;
using BitterECS.Core;

[Serializable]
public class ListActionComponent
{
    public struct AbilityEntry
    {
        public TagActions data;
        public EcsEntity entity;
    }

    public List<AbilityEntry> abilities = new();

    public void AddAbility(TagActions ability, EcsEntity abilityEntity)
    {
        if (ability.ability != null)
        {
            abilities.Add(new AbilityEntry { data = ability, entity = abilityEntity });
        }
    }

    public void RemoveAbility(TagActions ability)
    {
        abilities.RemoveAll(x => x.data.ability == ability.ability);
    }

    public bool Is<T>() where T : IActionAbility
    {
        if (abilities == null) return false;

        foreach (var entry in abilities)
        {
            if (entry.data.ability is T) return true;
        }
        return false;
    }

    public bool Is<T>(out T result) where T : IActionAbility
    {
        result = default;
        if (abilities == null) return false;

        foreach (var entry in abilities)
        {
            if (entry.data.ability is T t)
            {
                result = t;
                return true;
            }
        }
        return false;
    }

    public bool Is<T>(out T result, out EcsEntity abilityEntity) where T : IActionAbility
    {
        result = default;
        abilityEntity = default;

        foreach (var entry in abilities)
        {
            if (entry.data.ability is T t)
            {
                result = t;
                abilityEntity = entry.entity;
                return true;
            }
        }
        return false;
    }
}
