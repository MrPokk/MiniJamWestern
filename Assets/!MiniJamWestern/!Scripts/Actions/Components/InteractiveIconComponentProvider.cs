
using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct InteractiveIconComponent
{
    public InteractiveElementProvider iconSprite;
    public Sprite move;
    public Sprite attack;
    public Sprite rotation;
}


public class InteractiveIconComponentProvider : ProviderEcs<InteractiveIconComponent>
{
    protected override void Registration()
    {
        var interactiveElementProvider = GetComponentInChildren<InteractiveElementProvider>();
        Value.iconSprite ??= interactiveElementProvider;
        interactiveElementProvider.gameObject.SetActive(false);
    }
}
