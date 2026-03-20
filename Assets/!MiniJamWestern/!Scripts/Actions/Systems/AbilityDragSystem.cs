using BitterECS.Core;
using UnityEngine;

public class AbilityDragSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<DragAbilityEvent>(added: OnDrag);

    private static void OnDrag(EcsEntity entity)
    {
        if (!entity.Has<IsDraggingAbility>()) return;
        if (!entity.Has<PhysicsDragComponent>()) return;

        var dragEvent = entity.Get<DragAbilityEvent>();
        var view = entity.GetProvider<AbilityViewProvider>();
        if (view != null)
        {
            var worldPos = view.GetCamera().ScreenToWorldPoint(dragEvent.screenPosition);
            worldPos.z = 0;

            ref var physicsDrag = ref entity.Get<PhysicsDragComponent>();
            physicsDrag.targetWorldPosition = worldPos;
        }
    }
}

public class AbilityDragUpdateSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<IsDraggingAbility> _dragFilter = new();

    public void Run()
    {
        var pointerPos = ControllableSystem.PointerPosition;
        _dragFilter.For((EcsEntity entity, ref IsDraggingAbility drag) =>
        {
            entity.AddFrame(new DragAbilityEvent { screenPosition = pointerPos });
        });
    }
}


public class AbilityEndDragSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<PointerUpAbilityEvent>(added: OnPointerUp);

    private static void OnPointerUp(EcsEntity entity)
    {
        if (!entity.Has<IsDraggingAbility>()) return;

        var view = entity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        Vector2 worldPos = ControllableSystem.GetPointerPositionWorld();
        var hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.TryGetComponent<AbilitySlotProvider>(out var slot))
        {
            slot.AddItem(view);
        }

        view.EnableCollider(true);

        entity.Remove<IsDraggingAbility>();
        entity.Remove<PhysicsDragComponent>();
    }
}
