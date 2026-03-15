using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class WorldDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Camera _camera;
    private Vector3 _offset;

    private void Awake() => _camera = Camera.main;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        _offset = transform.position - mousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        transform.position = mousePos + _offset;
    }

    public void OnEndDrag(PointerEventData eventData) { }
}
