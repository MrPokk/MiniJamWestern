using System;
using BitterECS.Core;

public struct UpdateUIMoneyEvent { }
public struct PlayerUpdateMoneyUIEvent { }


public class MoneyVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _genericEvent;
    private EcsEvent _playerUIFilterEvent;

    private EcsFilter<TagPlayer, GridComponent> _ecsEntitiesPlayer;
    private EcsFilter<TagPlayerMoney, MoneyDisplayComponent, MoneyComponent> _ecsEntities;


    public void Init()
    {
        _genericEvent.Subscribe<UpdateUIMoneyEvent>(added: OnUpdateUI);
        _playerUIFilterEvent.Subscribe<PlayerUpdateMoneyUIEvent>(added: OnUpdateUIPlayer);
    }

    private void OnUpdateUIPlayer(EcsEntity entity)
    {
        NotifyPlayerUI();
    }

    private void OnUpdateUI(EcsEntity entity)
    {
        if (entity.TryGet<MoneyComponent>(out var money) &&
            entity.TryGet<MoneyDisplayComponent>(out var display))
        {
            Set(display, money.GetCurrentMoney());
        }
    }

    private void NotifyPlayerUI()
    {
        var player = _ecsEntitiesPlayer.First();
        var moneyPlayer = _ecsEntities.First();
        if (!player.IsAlive)
        {
            return;
        }

        if (moneyPlayer.TryGet<MoneyComponent>(out var money))
        {
            ref var display = ref moneyPlayer.Get<MoneyDisplayComponent>();
            Set(display, money.GetCurrentMoney());
        }
    }

    private static void Set(MoneyDisplayComponent display, int current)
    {
        if (display.slots == null) return;

        for (var i = 0; i < display.slots.Count; i++)
        {
            var slot = display.slots[i];
            if (slot == null) continue;

            if (i < current)
            {
                slot.SetActive(true);
                slot.SetSprite(display.full);
            }
            else
            {
                slot.SetActive(false);
            }
        }
    }
}
