using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISettingPopup : UIPopup, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image image;
    public Sprite selectSprite;

    private Sprite _defaultSprite;

    private void Awake()
    {
        if (image != null)
        {
            _defaultSprite = image.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null && selectSprite != null)
        {
            image.sprite = selectSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null && _defaultSprite != null)
        {
            image.sprite = _defaultSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIController.TryGetOpenedPopup<UIObjectSetting>(out var objectSetting))
        {
            UIController.ClosePopup<UIObjectSetting>();
        }
        else
        {
            if (objectSetting != null)
            {
                objectSetting.Open();
            }

            UIController.OpenPopup<UIObjectSetting>();
        }
    }
}
