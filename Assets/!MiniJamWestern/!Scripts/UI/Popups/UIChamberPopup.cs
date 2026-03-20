using System;
using System.Collections.Generic;
using BitterECS.Core;
using TMPro;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private List<Image> _healthSlots;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    public static Action<int, int> OnPlayerHealthChanged;

    public override void Open()
    {
        _nextTurnBtn.onClick.AddListener(OnNext);
        GFlow.OnTransferProgressChanged += UpdateTransferProgress;
        OnPlayerHealthChanged += UpdatePlayerHealth;

        UpdateTransferProgress(GFlow.GState.TransferProgress, GFlow.GState.TransferProgressMax);
        UpdateHealthFromECS();

        base.Open();
    }

    private void UpdateTransferProgress(int current, int max)
    {
        _progressText.text = $"{current} / {max}";
    }

    private void UpdatePlayerHealth(int current, int max)
    {
        for (var i = 0; i < _healthSlots.Count; i++)
        {
            var slot = _healthSlots[i];
            if (slot == null) continue;

            if (i < max)
            {
                slot.gameObject.SetActive(true);
                slot.sprite = (i < current) ? _fullHeart : _emptyHeart;
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
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
        GFlow.OnTransferProgressChanged -= UpdateTransferProgress;
        OnPlayerHealthChanged -= UpdatePlayerHealth;

        base.Close();
    }
}
