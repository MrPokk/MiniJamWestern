using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UISlotElement : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [SerializeField] private Color gizmoColor = Color.green;
    public GameObject currentItem;

    private RectTransform _rectTransform;

    private void Awake() => _rectTransform = GetComponent<RectTransform>();

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
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        Gizmos.color = gizmoColor;
        var corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);
        var center = _rectTransform.position;
        var size = new Vector3(Vector3.Distance(corners[0], corners[1]), Vector3.Distance(corners[1], corners[2]), 0);
        Gizmos.matrix = Matrix4x4.TRS(center, _rectTransform.rotation, _rectTransform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
