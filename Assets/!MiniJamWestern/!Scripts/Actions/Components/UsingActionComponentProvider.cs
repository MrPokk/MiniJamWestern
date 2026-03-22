using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public class UsingActionComponent
{
    public List<UsingAbilityElement> abilities = new();
}
[Serializable]
public class UsingAbilityElement
{
    public TagActionsProvider tagActionsProvider;
    public bool isVisibleInScene;
}

[RequireComponent(typeof(AbilityInventoryProvider))]
public class UsingActionComponentProvider : ProviderEcs<UsingActionComponent>
{
    private void Start() => FillAbilities();

    private void FillAbilities()
    {
        var entity = Entity;
        if (!entity.TryGet<AbilityInventory>(out var inventory)) return;

        var slots = inventory.listSlot;
        var slotIndex = 0;

        foreach (var element in Value.abilities)
        {
            var provider = element?.tagActionsProvider;
            if (provider == null) continue;

            if (!element.isVisibleInScene)
            {
                entity.GetOrAdd<ListActionComponent>().AddAbility(provider.Value, provider.Entity);
                continue;
            }

            if (slots != null && slotIndex < slots.Count)
            {
                var view = Instantiate(provider.gameObject).GetComponent<AbilityViewProvider>();
                if (view != null) slots[slotIndex++].AddItem(view);
            }
        }
    }
}
