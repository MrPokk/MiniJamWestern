using System;
using System.Collections.Generic;
using BitterECS.Core;
using BitterECS.Integration.Unity;

[Serializable]
public class AbilityInventory
{
    public List<AbilitySlotProvider> listSlot;
}

public class AbilityInventoryProvider : ProviderEcs<AbilityInventory>
{
    protected override void Registration()
    {
        FindAllSlot();
    }

    private void FindAllSlot()
    {
        Value.listSlot = new List<AbilitySlotProvider>(GetComponentsInChildren<AbilitySlotProvider>());
        foreach (var slot in Value.listSlot)
        {
            slot.Entity.Get<AbilitySlotComponent>().abilityInventory = this;
        }
    }

    public void ExtractAll()
    {
        foreach (var slot in Value.listSlot)
        {
            slot.RemoveItem();
        }
    }
}
