
using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public class UsingActionComponent
{
    public List<TagActionsProvider> abilities = new();
}

[RequireComponent(typeof(AbilityInventoryProvider))]
public class UsingActionComponentProvider : ProviderEcs<UsingActionComponent>
{
    protected override void PostRegistration()
    {
        FillSlotsWithAbilities(GetComponent<AbilityInventoryProvider>());
    }

    private void FillSlotsWithAbilities(AbilityInventoryProvider containerProvider)
    {
        ref var slots = ref containerProvider.Entity.Get<AbilityInventory>().listSlot;
        ref var abilities = ref Value.abilities;

        for (var i = 0; i < abilities.Count && i < slots.Count; i++)
        {
            var abilityPrefab = abilities[i].gameObject;
            var instance = Instantiate(abilityPrefab);

            var provider = instance.GetComponent<AbilityViewProvider>();

            slots[i].AddItem(provider);
        }
    }
}
