using System;
using System.Collections.Generic;
using BitterECS.Core;
using DG.Tweening;
using TMPro;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private List<Image> _healthSlots;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    public static Action<int, int> OnPlayerHealthChanged;

    private int _lastCurrentHealth = -1;
    private int _lastMaxHealth = -1;

    public override void Open()
    {
        _nextTurnBtn.onClick.AddListener(OnNext);
        OnPlayerHealthChanged += UpdatePlayerHealth;

        UpdateHealthFromECS();

        base.Open();
    }

    private void UpdatePlayerHealth(int current, int max)
    {
        for (var i = 0; i < _healthSlots.Count; i++)
        {
            var slot = _healthSlots[i];
            if (slot == null) continue;

            var isNowActive = i < max;
            if (isNowActive)
            {
                slot.gameObject.SetActive(true);

                var isNowFull = i < current;

                slot.sprite = isNowFull ? _fullHeart : _emptyHeart;

                slot.transform.DOKill();
                var originalScale = Vector3.one;
                slot.transform.localScale = Vector3.zero;
                slot.transform.DOScale(originalScale, 0.3f)
                    .SetEase(Ease.OutBack)
                    .Play();
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }

        _lastCurrentHealth = current;
        _lastMaxHealth = max;
    }

    private void UpdateHealthFromECS()
    {
        var playerFilter = new EcsFilter<TagPlayer>();
        var player = playerFilter.First();
        if (player.IsAlive && player.TryGet<HealthComponent>(out var health))
        {
            UpdatePlayerHealth(health.GetCurrentHealth(), health.GetMaxHealth());
        }
    }

    private void OnNext() => GFlow.RefreshTurn();

    public override void Close()
    {
        _nextTurnBtn.onClick.RemoveListener(OnNext);
        OnPlayerHealthChanged -= UpdatePlayerHealth;

        base.Close();
    }
}
