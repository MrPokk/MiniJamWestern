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
    [Header("UI References")][SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _titleLabel; [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private TMP_Text _amountLabel;

    [Header("Settings")]
    [SerializeField] private float _hoverOffset = 50f; [SerializeField] private float _hoverScale = 1.05f;
    [SerializeField] private float _animDuration = 0.25f;
    [SerializeField] private Ease _easeType = Ease.OutCubic;

    private CardType _type;
    public CardType Type => _type;
    public Action<ShopCard> onSelected;
    public int Price { get; private set; }

    private Vector2 _basePosition;
    private Quaternion _baseRotation = Quaternion.identity;
    private float _currentHoverY = 0f;
    private float _currentScale = 1f;

    public void AssignAction(TagActionsProvider provider)
    {
        if (provider == null) return;
        if (!provider.Entity.TryGet<SoldInfoComponent>(out var soldInfo)) return;

        _type = CardType.ACTION;
        if (soldInfo.icon != null) _icon.sprite = soldInfo.icon;

        _titleLabel.text = soldInfo.title;
        SetPrice(soldInfo.amount);

        var dynamicValue = 0;
        if (provider.Entity.TryGet<TagActions>(out var tagActions) && tagActions.ability is IComponentValue valueComp)
        {
            dynamicValue = valueComp.value;
        }

        _descriptionLabel.text = AbilityUIConverter.GetFinalText(soldInfo, dynamicValue);
    }

    public void AssignHeal(int amount, int defaultPrice = 50)
    {
        _type = CardType.HEAL;
        _titleLabel.text = "Heal";
        _descriptionLabel.text = $"Restore {amount} HP";
        SetPrice(defaultPrice);
    }

    public void SetPrice(int price)
    {
        Price = price;
        _amountLabel.text = Price.ToString();
    }

    public void SetArcTransform(Vector2 basePosition, Quaternion baseRotation)
    {
        _basePosition = basePosition;
        _baseRotation = baseRotation;
        ApplyTransform();
    }

    private void ApplyTransform()
    {
        transform.SetLocalPositionAndRotation(_basePosition + new Vector2(0, _currentHoverY), _baseRotation);
        transform.localScale = new Vector3(_currentScale, _currentScale, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(this);

        DOTween.To(() => _currentHoverY, x => _currentHoverY = x, _hoverOffset, _animDuration)
            .SetEase(_easeType)
            .SetTarget(this)
            .OnUpdate(ApplyTransform)
            .Play();

        DOTween.To(() => _currentScale, x => _currentScale = x, _hoverScale, _animDuration)
            .SetEase(_easeType)
            .SetTarget(this)
            .OnUpdate(ApplyTransform)
            .Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(this);

        DOTween.To(() => _currentHoverY, x => _currentHoverY = x, 0f, _animDuration)
            .SetEase(_easeType)
            .SetTarget(this)
            .OnUpdate(ApplyTransform)
            .Play();

        DOTween.To(() => _currentScale, x => _currentScale = x, 1f, _animDuration)
            .SetEase(_easeType)
            .SetTarget(this)
            .OnUpdate(ApplyTransform)
            .Play();
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }
}
