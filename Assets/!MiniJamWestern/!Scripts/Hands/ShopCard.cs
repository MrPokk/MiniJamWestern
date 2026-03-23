using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class ShopCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CardType { ACTION, PERK, HEAL }

    [Header("UI References")]
    public RectTransform visual;
    public Image topIcon;
    public Image bottomIcon;
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI descriptionLabel;

    [Header("Settings")]
    public float hoverOffset = 20f;
    public float animDuration = 0.2f;

    public CardType type;
    public object data; // Сюда кладем Perk, Action или количество хила
    public Action<ShopCard> OnSelected;

    private Vector2 _basePos;

    void Start() => _basePos = visual.anchoredPosition;

    // Инициализация для Способностей (Actions)
    public void AssignActions(TagActions top, TagActions bottom)
    {
        type = CardType.ACTION;
        data = new[] { top, bottom };
        titleLabel.text = "Ability Bundle";
        descriptionLabel.text = $"Top: {top.ability.GetType().Name}\nBottom: {bottom.ability.GetType().Name}";
        // topIcon.sprite = ... (если есть иконки)
    }

    // Инициализация для Перков
    public void AssignPerk(object perkData, string name, string desc)
    {
        type = CardType.PERK;
        data = perkData;
        titleLabel.text = name;
        descriptionLabel.text = desc;
        bottomIcon.gameObject.SetActive(false);
    }

    // Инициализация для Хила
    public void AssignHeal(int amount)
    {
        type = CardType.HEAL;
        data = amount;
        titleLabel.text = "Heal";
        descriptionLabel.text = $"Restore {amount} HP";
        bottomIcon.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelected?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        visual.DOAnchorPosY(_basePos.y + hoverOffset, animDuration);
        visual.DOScale(1.05f, animDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        visual.DOAnchorPosY(_basePos.y, animDuration);
        visual.DOScale(1f, animDuration);
    }
}
