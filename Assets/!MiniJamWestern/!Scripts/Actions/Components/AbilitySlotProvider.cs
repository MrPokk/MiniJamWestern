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

    public bool AddItem(AbilityViewProvider item)
    {
        if (Value.itemEntity.IsAlive) return false;

        var ownerInventory = Value.abilityInventory.Entity;
        var systemView = EcsSystemStatic.GetSystem<AbilitySlotViewSystem>();
        var system = EcsSystemStatic.GetSystem<AbilitySlotSystem>();

        if (system.Placing(ownerInventory, Entity, item.Entity))
        {
            var result = systemView.Placing(item, this);
            Value.abilityInventory.UpdateVisibility();
            return result;
        }

        return false;
    }

    public bool TryRemoveItem()
    {
        var removedItem = Value.itemEntity;
        if (!removedItem.IsAlive) return false;

        var ownerInventory = Value.abilityInventory.Entity;
        var systemView = EcsSystemStatic.GetSystem<AbilitySlotViewSystem>();
        var system = EcsSystemStatic.GetSystem<AbilitySlotSystem>();

        if (system.Extract(ownerInventory, Entity, removedItem))
        {
            var result = systemView.Extract(this);
            Value.abilityInventory.UpdateVisibility();
            return result;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent<BoxCollider2D>(out var boxCollider)) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.offset;

        var gizmoColor = Color.green;
        var fillColor = gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        var edgeColor = gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(center, size);
    }
}
