using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UISlotElement : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] private Color _gizmoColor = Color.green;
    public GameObject currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (currentItem == null && eventData.pointerDrag != null)
        {
            currentItem = eventData.pointerDrag;
            currentItem.transform.SetParent(transform);

            if (currentItem.TryGetComponent<RectTransform>(out var dragRect))
            {
                dragRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            currentItem.transform.SetParent(transform.root);
            currentItem = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        var fillColor = _gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        var edgeColor = _gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
