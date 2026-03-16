using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public class ContainerActions
{
    public List<DraggableSlot> listSlot;
}

public class ContainerActionsProvider : ProviderEcs<ContainerActions>
{
    private void Start()
    {
        var childSlots = GetComponentsInChildren<DraggableSlot>();

        Value.listSlot = new List<DraggableSlot>(childSlots);

        foreach (var slot in Value.listSlot)
        {
            if (slot != null)
            {
                slot.OnItemAdded += HandleItemAdded;
                slot.OnItemRemoved += HandleItemRemoved;
            }
        }
    }

    protected override void OnDestroy()
    {
        if (Value.listSlot == null) return;

        foreach (var slot in Value.listSlot)
        {
            if (slot != null)
            {
                slot.OnItemAdded -= HandleItemAdded;
                slot.OnItemRemoved -= HandleItemRemoved;
            }
        }

        base.OnDestroy();
    }

    private void HandleItemAdded(DraggableComponentProvider item)
    {
        var tagActions = item.Entity.Get<TagActions>();
        var ability = tagActions.ability;
        Entity.AddFrame<IsTargetingActionEnterEvent>(new(ability));
    }

    private void HandleItemRemoved(DraggableComponentProvider item)
    {
        var tagActions = item.Entity.Get<TagActions>();
        var ability = tagActions.ability;
        Entity.AddFrame<IsTargetingActionExitEvent>(new(ability));
    }
}
