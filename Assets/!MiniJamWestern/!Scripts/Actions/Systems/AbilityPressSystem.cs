using BitterECS.Core;
using UnityEngine;

public class AbilityLongPressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<LongPressAbilityEvent>(added: OnLongPress);

    private static void OnLongPress(EcsEntity abilityEntity)
    {
        Debug.Log($"[AbilityLongPressSystem] Long press detected on ability entity: {abilityEntity}");

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
        Debug.Log($"[AbilityPressSystem] Pointer down on entity: {entity} at {Time.time}");
        var ev = entity.Get<PointerDownAbilityEvent>();
        entity.Add(new PointerDownAbility { pressTime = ev.pressTime });
    }

    private static void OnPointerUp(EcsEntity entity)
    {
        Debug.Log($"[AbilityPressSystem] Pointer up on entity: {entity} at {Time.time}");
        if (entity.Has<PointerDownAbility>())
        {
            Debug.Log($"[AbilityPressSystem] Short press detected on entity: {entity}");
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
                Debug.Log($"[AbilityPressSystem] Long press detected on entity: {entity}");
                entity.AddFrame<LongPressAbilityEvent>();
                entity.Remove<PointerDownAbility>();
            }
        });
    }
}
