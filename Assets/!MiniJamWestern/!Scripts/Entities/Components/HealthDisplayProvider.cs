using System;
using System.Collections.Generic;
using System.Linq;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;


[Serializable]
public struct HealthDisplayComponent
{
    public Sprite full;
    public Sprite empty;
    public List<HealthElementProvider> slots;
}

public class HealthDisplayProvider : ProviderEcs<HealthDisplayComponent>
{
    protected override void PostRegistration()
    {
        Value.slots = GetComponentsInChildren<HealthElementProvider>(true).ToList();
    }

    private void Start()
    {
        Entity.AddFrame<UpdateHealthUIEvent>();
    }
}
