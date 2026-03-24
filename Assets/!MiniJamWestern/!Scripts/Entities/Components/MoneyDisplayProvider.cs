using System;
using System.Collections.Generic;
using System.Linq;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;


[Serializable]
public struct MoneyDisplayComponent
{
    public Sprite full;
    public Sprite empty;
    public List<IDisplay> slots;
}

public class MoneyDisplayProvider : ProviderEcs<MoneyDisplayComponent>
{
    protected override void PostRegistration()
    {
        Value.slots = GetComponentsInChildren<IDisplay>(true).ToList();
    }
    private void Start()
    {
        Entity.AddFrame<UpdateUIEvent>();
    }
}
