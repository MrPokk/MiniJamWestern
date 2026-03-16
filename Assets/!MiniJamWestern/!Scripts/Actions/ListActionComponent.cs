using System.Collections.Generic;

public class ListActionComponent
{
    public List<IActionAbility> abilities = new List<IActionAbility>();

    public ListActionComponent() { }

    public ListActionComponent(IActionAbility ability)
    {
        if (ability != null)
        {
            abilities.Add(ability);
        }
    }

    public void AddAbility(IActionAbility ability)
    {
        if (ability != null && !abilities.Contains(ability))
        {
            abilities.Add(ability);
        }
    }

    public void RemoveAbility(IActionAbility ability)
    {
        if (ability != null)
        {
            abilities.Remove(ability);
        }
    }

    public bool Is<T>() where T : IActionAbility
    {
        foreach (var ability in abilities)
        {
            if (ability is T) return true;
        }
        return false;
    }

    public bool Is<T>(out T result) where T : IActionAbility
    {
        foreach (var ability in abilities)
        {
            if (ability is T t)
            {
                result = t;
                return true;
            }
        }

        result = default;
        return false;
    }
}
