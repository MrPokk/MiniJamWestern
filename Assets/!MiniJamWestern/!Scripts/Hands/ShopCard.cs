using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using BitterECS.Integration.Unity;

public class ShopCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CardType { ACTION, PERK, HEAL }

    [Header("UI References")]
    public RectTransform visual;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private TMP_Text _amountLabel;


    [Header("Settings")]
    [SerializeField] private float _hoverOffset = 40f;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _animDuration = 0.2f;

    private CardType _type;
    public CardType Type => _type;
    public Action<ShopCard> onSelected;

    private Tween _moveTween;
    private Tween _scaleTween;

    public void AssignAction(TagActionsProvider provider)
    {
        if (provider == null) return;

        if (!provider.Entity.TryGet<SoldInfoComponent>(out var soldInfo))
        {
            throw new("Component SoldInfoComponent not found on entity");
        }

        _type = CardType.ACTION;

        if (soldInfo.icon != null)
            _icon.sprite = soldInfo.icon;

        _titleLabel.text = soldInfo.title;
        _amountLabel.text = soldInfo.amount.ToString();

        var dynamicValue = 0;
        var ability = provider.Entity.Get<TagActions>().ability;

        if (ability is IComponentValue valueComp)
        {
            dynamicValue = valueComp.value;
        }

        _descriptionLabel.text = AbilityUIConverter.GetFinalText(soldInfo, dynamicValue);
    }

    public void AssignHeal(int amount)
    {
        _type = CardType.HEAL;
        _titleLabel.text = "Heal";
        _descriptionLabel.text = $"Restore {amount} HP";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();

        _moveTween = visual.DOAnchorPosY(_hoverOffset, _animDuration);
        _scaleTween = visual.DOScale(_hoverScale, _animDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _moveTween?.Kill();
        _scaleTween?.Kill();

        _moveTween = visual.DOAnchorPosY(0, _animDuration);
        _scaleTween = visual.DOScale(1f, _animDuration);
    }
}
