using System.Collections.Generic;
using BitterECS.Core;
using TMPro;
using UINotDependence.Core;
using UnityEngine;

public class UIShopPopup : UIPopup
{
    public Transform cardContainer;
    public TMP_Text amountCount;
    private EcsFilter<TagPlayerMoney, MoneyComponent> _ecsEntities;


    [SerializeField] private float _space = 200f;
    [SerializeField] private float _amplitude = 100f;
    [SerializeField] private float _frequency = 0.005f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private bool _animate = true;
    [SerializeField] private bool _rotateItemsToArc = true;

    private readonly List<ShopCard> _cards = new();

    public void Clear()
    {
        foreach (Transform child in cardContainer)
        {
            if (child != null) Destroy(child.gameObject);
        }
        _cards.Clear();
    }

    public void AddCard(ShopCard card)
    {
        card.transform.SetParent(cardContainer);
        _cards.Add(card);
        UpdateLayout();
        amountCount.text = _ecsEntities.First().Get<MoneyComponent>().GetCurrentMoney().ToString();
    }

    public override void Open()
    {
        UIAnimationComponent
        .Using(gameObject)
        .SetPresets(UIAnimationPresets.FadeIn,
                    UIAnimationPresets.FadeOut)
        .PlayOpen();

        base.Open();
    }

    private void Update() => UpdateLayout();

    private void UpdateLayout()
    {
        var count = _cards.Count;
        if (count == 0) return;

        var centerOffset = (count - 1) * 0.5f;
        var timeOffset = _animate ? Time.time * _speed : 0;

        for (var i = 0; i < count; i++)
        {
            var card = _cards[i];
            if (card == null) continue;

            var x = (i - centerOffset) * _space;
            var phase = x * _frequency + timeOffset;
            var y = Mathf.Sin(phase) * _amplitude;

            var targetPosition = new Vector2(x, y);
            var targetRotation = Quaternion.identity;

            if (_rotateItemsToArc)
            {
                var tangent = _amplitude * _frequency * Mathf.Cos(phase);
                var angle = Mathf.Atan(tangent) * Mathf.Rad2Deg;
                targetRotation = Quaternion.Euler(0, 0, angle);
            }

            card.SetArcTransform(targetPosition, targetRotation);
        }
    }
}
