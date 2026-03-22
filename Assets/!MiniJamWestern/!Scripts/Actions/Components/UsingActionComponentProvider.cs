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

    [SerializeField] private bool _hideSlots;

    private void Start() => FillAbilities();

    private void FillAbilities()
    {
        var entity = Entity;
        if (!entity.TryGet<AbilityInventory>(out var inventory)) return;

        var slots = inventory.listSlot;
        if (slots == null) return;

        foreach (var slot in slots)
        {
            if (slot != null && _hideSlots) slot.gameObject.SetActive(false);
        }

        var slotIndex = 0;
        var listAction = entity.GetOrAdd<ListActionComponent>();

        foreach (var element in Value.abilities)
        {
            var provider = element?.tagActionsProvider;
            if (provider == null) continue;

            if (!element.isVisibleInScene)
            {
                listAction.AddAbility(provider.Value, provider.Entity);
                continue;
            }

            if (slotIndex >= slots.Count)
            {
                continue;
            }

            if (!Instantiate(provider.gameObject).TryGetComponent<AbilityViewProvider>(out var view))
            {
                continue;
            }

            var targetSlot = slots[slotIndex];
            if (targetSlot.AddItem(view))
            {
                targetSlot.gameObject.SetActive(true);

                slotIndex++;
            }
            else
            {
                throw new("Unable to add ability to slot. Please check the");
            }
        }
    }
}
