using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class DraggableSlot : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] private Color _gizmoColor = Color.green;
    public DraggableComponentProvider currentItem;

    public event Action<DraggableComponentProvider> OnItemRemoved;
    public event Action<DraggableComponentProvider> OnItemAdded;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null ||
            !eventData.pointerDrag.TryGetComponent<DraggableComponentProvider>(out var draggable))
        {
            return;
        }

        TryAddItem(draggable);
    }

    public bool TryAddItem(DraggableComponentProvider item)
    {
        if (currentItem != null) return false;

        currentItem = item;
        currentItem.transform.SetParent(transform);
        currentItem.transform.localPosition = Vector3.zero;

        OnItemAdded?.Invoke(currentItem);
        return true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentItem == null) return;

        var removedItem = currentItem;

        currentItem.transform.SetParent(null);
        currentItem = null;

        OnItemRemoved?.Invoke(removedItem);
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent<BoxCollider2D>(out var boxCollider)) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        var size = (Vector3)boxCollider.size;
        var center = (Vector3)boxCollider.offset;

        var fillColor = _gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        var edgeColor = _gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(center, size);
    }
}
