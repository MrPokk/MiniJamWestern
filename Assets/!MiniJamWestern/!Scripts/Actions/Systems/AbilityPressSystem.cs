using BitterECS.Core;
using UnityEngine;

public class AbilityPressSystem : IEcsAutoImplement, IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<PointerDownAbility> _downFilter = new();
    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<PointerDownAbilityEvent>(added: OnPointerDown)
        .Subscribe<PointerUpAbilityEvent>(added: OnPointerUp);

    private const float HOLD_DURATION = 0.5f;

    private static void OnPointerDown(EcsEntity entity)
    {
        var ev = entity.Get<PointerDownAbilityEvent>();
        entity.Add(new PointerDownAbility { pressTime = ev.pressTime });
    }

    private static void OnPointerUp(EcsEntity entity)
    {
        if (entity.Has<PointerDownAbility>())
        {
            entity.AddFrame<ShortPressAbilityEvent>();
            entity.Remove<PointerDownAbility>();
        }
    }

    public void Run()
    {
        var now = Time.time;
        _downFilter.For((EcsEntity entity, ref PointerDownAbility down) =>
        {
            if (now - down.pressTime >= HOLD_DURATION)
            {
                entity.AddFrame<LongPressAbilityEvent>();
                entity.Remove<PointerDownAbility>();
            }
        });
    }
}
