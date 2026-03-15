using BitterECS.Integration.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableComponent { }

[RequireComponent(typeof(BoxCollider2D))]
public class DraggableComponentProvider : ProviderEcs<DraggableComponent>, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Color _gizmoColor = Color.green;
    private Camera _camera;
    private Collider2D _collider;

    private void Start()
    {
        _camera = Camera.main;
        _collider = GetComponent<Collider2D>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _collider.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePos = _camera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;

        transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _collider.enabled = true;
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
