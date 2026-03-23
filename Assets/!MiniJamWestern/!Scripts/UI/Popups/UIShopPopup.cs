using System.Collections.Generic;
using UINotDependence.Core;
using UnityEngine;

public class UIShopPopup : UIPopup
{
    public Transform cardContainer;

    [Header("Layout Settings")]
    [SerializeField] private float _space = 300f;
    [Range(100, 10000)]
    [SerializeField] private float _parabolaParameter = 2000f;
    [SerializeField] private bool _rotateItemsToArc = true;

    private readonly List<RectTransform> _cards = new();

    public void Clear()
    {
        foreach (Transform child in cardContainer) Destroy(child.gameObject);
        _cards.Clear();
    }

    public void AddCard(ShopCard card)
    {
        card.transform.SetParent(cardContainer);
        _cards.Add(card.GetComponent<RectTransform>());
        UpdateLayout();
    }

    private void Update() => UpdateLayout();

    private void UpdateLayout()
    {
        var count = _cards.Count;
        if (count == 0) return;

        var centerOffset = (count - 1) * 0.5f;
        var invParabola = 1f / _parabolaParameter;

        for (var i = 0; i < count; i++)
        {
            var rect = _cards[i];
            if (rect == null) continue;

            var linear = (i - centerOffset) * _space;
            var curve = -(linear * linear) * invParabola;

            rect.anchoredPosition = new Vector2(linear, curve);

            if (_rotateItemsToArc)
            {
                var tangent = -2f * linear * invParabola;
                var angle = Mathf.Atan(tangent) * Mathf.Rad2Deg;
                rect.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                rect.localRotation = Quaternion.identity;
            }
        }
    }
}
