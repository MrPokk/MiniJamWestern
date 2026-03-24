using System;
using BitterECS.Core;

public struct UpdateUIMoneyEvent { }

public class MoneyVisualSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _genericEvent;

    public void Init()
    {
        _genericEvent.Subscribe<UpdateUIMoneyEvent>(added: OnUpdateUI);
    }

    private void OnUpdateUI(EcsEntity entity)
    {
        if (entity.TryGet<MoneyComponent>(out var money) &&
            entity.TryGet<MoneyDisplayComponent>(out var display))
        {
            Set(display, money.GetCurrentMoney(), money.GetMaxMoney());
        }
    }

    private static void Set(MoneyDisplayComponent display, int current, int max)
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
