using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public struct AbilitySlotComponent
{
    public EcsEntity itemEntity;
    public AbilityInventoryProvider abilityInventory;
}

[RequireComponent(typeof(BoxCollider2D))]
public class AbilitySlotProvider : ProviderEcs<AbilitySlotComponent>, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<AbilityViewProvider>(out var draggable))
        {
            AddItem(draggable);
        }
    }

    public void AddItem(AbilityViewProvider item)
    {
        if (Value.itemEntity.IsAlive)
            return;

        item.Entity.Remove<IsDraggingAbility>();
        item.EnableCollider(true);

        Value.itemEntity = item.Entity;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        var ownerInventory = Value.abilityInventory.Entity;

        var system = EcsSystemStatic.GetSystem<AbilitySlotSystem>();
        system.Placing(ownerInventory, item.Entity);
    }

    public bool TryRemoveItem()
    {
        if (!Value.itemEntity.IsAlive)
            return false;

        var ownerInventory = Value.abilityInventory.Entity;
        var removedItem = Value.itemEntity;

        var provider = removedItem.GetProvider<ProviderEcs>();
        if (provider != null)
        {
            provider.transform.SetParent(null);
        }

        Value.itemEntity = EcsEntity.Null;

        var system = EcsSystemStatic.GetSystem<AbilitySlotSystem>();
        system.Extract(ownerInventory, removedItem);

        return true;
    }

    public void RemoveItem() => TryRemoveItem();

    private void OnDrawGizmos()
    {
        if (!TryGetComponent<BoxCollider2D>(out var boxCollider)) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.offset;

        Color gizmoColor = Color.green;
        Color fillColor = gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        Color edgeColor = gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(center, size);
    }
}
