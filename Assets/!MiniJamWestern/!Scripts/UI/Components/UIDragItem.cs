using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (transform.parent == transform.root)
        {
            // Здесь можно добавить логику возврата на старое место, если не попали в слот
        }
    }
}
