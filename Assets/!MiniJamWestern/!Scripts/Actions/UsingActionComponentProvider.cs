
using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public class UsingActionComponent
{
    public List<TagActionsProvider> abilities = new();
}

[RequireComponent(typeof(ContainerActionsProvider))]
public class UsingActionComponentProvider : ProviderEcs<UsingActionComponent>
{
    protected override void PostRegistration()
    {
        FillSlotsWithAbilities(GetComponent<ContainerActionsProvider>());
    }

    private void FillSlotsWithAbilities(ContainerActionsProvider containerProvider)
    {
        ref var slots = ref containerProvider.Entity.Get<ContainerActions>().listSlot;
        ref var abilities = ref Value.abilities;

        for (var i = 0; i < abilities.Count && i < slots.Count; i++)
        {
            var abilityPrefab = abilities[i].gameObject;
            var instance = Instantiate(abilityPrefab);

            var provider = instance.GetComponent<DraggableComponentProvider>();

            slots[i].TryAddItem(provider);
        }
    }
}
