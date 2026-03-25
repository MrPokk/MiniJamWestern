using System;
using BitterECS.Core;
using UnityEngine;

public struct UpdateHealthUIEvent { }
public struct PlayerUpdateHealthUIEvent { }

public class HealthVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _genericEvent;
    private EcsEvent _playerUIFilterEvent;
    private EcsFilter<TagPlayer, GridComponent> _playerFilter;

    public void Init()
    {
        _genericEvent.SubscribeAny<UpdateHealthUIEvent, IsDamagedEvent>(added: OnUpdateUI);
        _playerUIFilterEvent.Subscribe<PlayerUpdateHealthUIEvent>(added: OnUpdateUIPlayer);
    }

    private void OnUpdateUIPlayer(EcsEntity entity)
    {
        NotifyPlayerUI();
    }

    private void OnUpdateUI(EcsEntity entity)
    {
        if (EcsConditions.Has<TagPlayer, GridComponent>(entity))
        {
            NotifyPlayerUI();
            return;
        }

        if (entity.TryGet<HealthComponent>(out var health) &&
            entity.TryGet<HealthDisplayComponent>(out var display))
        {
            SetHealth(display, health.GetCurrentHealth(), health.GetMaxHealth());
        }
    }

    private void NotifyPlayerUI()
    {
        var player = _playerFilter.First();
        if (player.IsAlive)
        {
            if (!player.TryGet<HealthComponent>(out var health))
            {
                return;
            }

            Debug.Log($"Player add heath {health.GetMaxHealth()}   {health.GetCurrentHealth()}");
            UIChamberPopup.OnPlayerHealthChanged?.Invoke(health.GetCurrentHealth(), health.GetMaxHealth());
        }
        else
            Debug.Log("Player add heath");
    }

    private static void SetHealth(HealthDisplayComponent display, int current, int max)
    {
        if (display.slots == null) return;

        for (var i = 0; i < display.slots.Count; i++)
        {
            var slot = display.slots[i];
            if (slot == null) continue;

            if (i < max)
            {
                slot.SetActive(true);
                slot.SetSprite(i < current ? display.full : display.empty);
            }
            else
            {
                slot.SetActive(false);
            }
        }
    }
}
