using BitterECS.Core;
using UnityEngine;
public class AbilityLongPressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<LongPressAbilityEvent>(added: OnLongPress);

    private static void OnLongPress(EcsEntity abilityEntity)
    {
        if (abilityEntity.Has<IsDraggingAbility>()) return;

        var view = abilityEntity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        var parentSlot = view.GetComponentInParent<AbilitySlotProvider>();
        if (parentSlot != null)
        {
            var ownerEntity = parentSlot.Value.abilityInventory.Entity;
            if (ownerEntity.Has<IsNotDragging>() || parentSlot.Entity.Has<IsNotDragging>())
            {
                Debug.Log($"Dragging blocked by inventory or slot constraints.");
                return;
            }

            if (parentSlot.Value.itemEntity == abilityEntity)
            {
                if (parentSlot.TryRemoveItem())
                {
                    abilityEntity.Add(new DraggingFromSlotComponent { slotProvider = parentSlot });
                }
            }
        }

        view.EnableCollider(false);
        abilityEntity.Add<IsDraggingAbility>();

        abilityEntity.Add(new PhysicsDragComponent
        {
            targetWorldPosition = view.transform.position,
            currentVelocity = Vector3.zero
        });
    }
}

public class AbilityPressSystem : IEcsAutoImplement, IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<PointerDownAbility> _downFilter = new();
    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<PointerDownAbilityEvent>(added: OnPointerDown)
        .Subscribe<PointerUpAbilityEvent>(added: OnPointerUp);

    private const float HOLD_DURATION = 0.3f;

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
