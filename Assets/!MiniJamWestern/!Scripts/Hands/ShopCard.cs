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

        //type = CardType.ACTION;
        //data = provider;
        //titleLabel.text = provider.name;
        //// Настройку иконок и описания можно добавить сюда
        //bottomIcon.gameObject.SetActive(true);
    }

    //// Инициализация для Перков
    //public void AssignPerk(object perkData, string name, string desc)
    //{
    //    type = CardType.PERK;
    //    data = perkData;
    //    titleLabel.text = name;
    //    descriptionLabel.text = desc;
    //    bottomIcon.gameObject.SetActive(false);
    //}

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
