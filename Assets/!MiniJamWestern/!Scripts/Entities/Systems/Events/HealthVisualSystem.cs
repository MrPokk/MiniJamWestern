using System;
using BitterECS.Core;

public struct UpdateHealthUIEvent { }
public struct PlayerUpdateHealthUIEvent { }

public class HealthVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _genericEvent;
    private EcsEvent _playerUIFilterEvent;
    private EcsFilter<UITagPlayerHealth> _playerUIFilter;
    private EcsFilter<TagPlayer> _player;


    public void Init()
    {
        _genericEvent.SubscribeAny<UpdateHealthUIEvent, IsDamagedEvent>(added: OnUpdateUI);
        _playerUIFilterEvent.Subscribe<PlayerUpdateHealthUIEvent>(added: OnUpdateUIPlayer);
    }

    private void OnUpdateUIPlayer(EcsEntity entity)
    {
        var playerEntity = _player.First();
        PlayerUpdateUI(playerEntity);
    }

    private void OnUpdateUI(EcsEntity entity)
    {
        PlayerUpdateUI(entity);

        if (entity.TryGet<HealthComponent>(out var health) &&
            entity.TryGet<HealthDisplayComponent>(out var display))
        {
            SetHealth(display, health.GetCurrentHealth(), health.GetMaxHealth());
        }
    }

    private void PlayerUpdateUI(EcsEntity entity)
    {
        if (!entity.Has<TagPlayer>())
            return;

        if (!entity.TryGet<HealthComponent>(out var healthPlayer))
            return;

        var current = healthPlayer.GetCurrentHealth();
        var max = healthPlayer.GetMaxHealth();
        var uiPlayer = _playerUIFilter.First();

        SetHealth(uiPlayer.Get<HealthDisplayComponent>(), current, max);

        return;
    }

    private static void SetHealth(HealthDisplayComponent display, int current, int max)
    {
        if (display.slots == null)
            return;

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
