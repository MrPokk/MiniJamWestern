using System;
using System.Collections.Generic;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public class HealthDisplay
{
    public Sprite full;
    public Sprite empty;
    public List<HealthElementProvider> listSlot;
}

public class HealthDisplayProvider : ProviderEcs<HealthDisplay>
{
    protected override void PostRegistration()
    {
        FindAllSlot();
    }

    private void Start()
    {
        Entity.AddFrame<UpdateHealthUIEvent>();
    }

    private void FindAllSlot()
    {
        Value.listSlot = new List<HealthElementProvider>(GetComponentsInChildren<HealthElementProvider>());
        foreach (var element in Value.listSlot)
        {
            element.Entity.Get<HealthElementComponent>().healthDisplay = this;
        }
    }

}
