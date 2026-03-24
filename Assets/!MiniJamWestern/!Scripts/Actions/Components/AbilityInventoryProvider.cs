using System;
using System.Collections.Generic;
using System.Linq;
using BitterECS.Core;
using BitterECS.Integration.Unity;

[Serializable]
public struct AbilityInventory
{
    public List<AbilitySlotProvider> listSlot;
}

public class AbilityInventoryProvider : ProviderEcs<AbilityInventory>
{
    protected override void Registration()
    {
        FindAllSlot();
        UpdateVisibility();
    }

    public void AddFirstEmpty(AbilityViewProvider item)
    {
        foreach (var slot in Value.listSlot)
        {
            if (slot.gameObject.activeSelf && slot.AddItem(item))
            {
                return;
            }
        }
    }

    private void FindAllSlot()
    {
        Value.listSlot = new List<AbilitySlotProvider>(GetComponentsInChildren<AbilitySlotProvider>(true));
        foreach (var slot in Value.listSlot)
        {
            slot.Entity.Get<AbilitySlotComponent>().abilityInventory = this;
        }
    }

    public void ExtractAll()
    {
        foreach (var slot in Value.listSlot)
        {
            slot.TryRemoveItem();
        }
    }

    public void UpdateVisibility()
    {
        var groupedSlots = Value.listSlot.GroupBy(s => s.transform.GetSiblingIndex());
        foreach (var group in groupedSlots)
        {
            bool showNext = true;
            foreach (var slot in group)
            {
                slot.gameObject.SetActive(showNext);
                showNext = slot.Entity.Get<AbilitySlotComponent>().itemEntity.IsAlive;
            }
        }
    }
}
