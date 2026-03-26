using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;

public class ShopCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CardType { ACTION, PERK, HEAL, MAX_HEALTH }

    [Header("UI References")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _descriptionLabel;
    [SerializeField] private TMP_Text _amountLabel;
    [SerializeField] private TMP_Text _amountFreeLabel;
    [SerializeField] private GameObject _amountObject;

    [Header("UI Health References")]
    [SerializeField] private Sprite _healthMax;
    [SerializeField] private Sprite _healthRegent;

    [Header("Settings")][SerializeField] private float _hoverOffset = 50f;
    [SerializeField] private float _hoverScale = 1.05f;
    [SerializeField] private float _animDuration = 0.25f;
    [SerializeField] private Ease _easeType = Ease.OutCubic; [Header("Setting Affordable")]
    [SerializeField] private ShimmeringShaderController _shimmeringShaderController;
    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    private CardType _type;
    public CardType Type => _type;
    public Action<ShopCard> onSelected;
    public int Price { get; private set; }

    public AbilityViewProvider AbilityView { get; private set; }
    public int HealthAmount { get; private set; }

    private Vector2 _basePosition;
    private Quaternion _baseRotation = Quaternion.identity;
    private float _currentHoverY = 0f;
    private float _currentScale = 1f;

    private void Awake()
    {
        _amountFreeLabel.gameObject.SetActive(false);

        _shimmeringShaderController ??= GetComponentInChildren<ShimmeringShaderController>();
    }

    public void SetAffordable()
    {
        if (!_shimmeringShaderController)
            throw new ArgumentNullException(nameof(_shimmeringShaderController) + "is not set");

        _shimmeringShaderController.Color1 = _color1;
        _shimmeringShaderController.Color2 = _color2;
    }

    public void AssignAction(TagActionsProvider provider)
    {
        if (provider == null) return;
        if (!provider.Entity.TryGet<SoldInfoComponent>(out var soldInfo)) return;

        _type = CardType.ACTION;
        AbilityView = provider.Entity.GetProvider<AbilityViewProvider>();

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

    public void AssignHeal(int amount, int defaultPrice)
    {
        _type = CardType.HEAL;
        if (_healthRegent != null) _icon.sprite = _healthRegent;
        _titleLabel.text = "Heal";
        _descriptionLabel.text = $"Restore {amount} HP";
        HealthAmount = amount;
        SetPrice(defaultPrice);
    }

    public void AssignMaxHealth(int amount, int defaultPrice)
    {
        _type = CardType.MAX_HEALTH;
        if (_healthMax != null) _icon.sprite = _healthMax;
        _titleLabel.text = "Max HP";
        _descriptionLabel.text = $"+{amount} Max HP";
        HealthAmount = amount;
        SetPrice(defaultPrice);
    }

    public void SetPrice(int price)
    {
        if (price <= 0)
        {
            _amountObject.SetActive(false);
            _amountFreeLabel.gameObject.SetActive(true);
        }

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
