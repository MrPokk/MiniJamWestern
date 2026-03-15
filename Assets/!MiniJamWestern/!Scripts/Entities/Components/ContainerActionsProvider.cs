using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;

[Serializable]
public class ContainerActions
{
    public List<DraggableSlot> listAction;
}

public class ContainerActionsProvider : ProviderEcs<ContainerActions>
{
    private void Start()
    {
        if (Value.listAction == null) return;

        foreach (var slot in Value.listAction)
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
        if (Value.listAction == null) return;

        foreach (var slot in Value.listAction)
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
