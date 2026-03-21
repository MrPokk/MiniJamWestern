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
            var camera = view.GetCamera();
            var worldPos = camera.ScreenToWorldPoint(dragEvent.screenPosition);
            worldPos.z = 0;

            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            var topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, camera.nearClipPlane));
            worldPos.x = Mathf.Clamp(worldPos.x, bottomLeft.x, topRight.x);
            worldPos.y = Mathf.Clamp(worldPos.y, bottomLeft.y, topRight.y);

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

        var placed = false;

        if (hit != null && hit.TryGetComponent<AbilitySlotProvider>(out var slot))
        {
            slot.AddItem(view);
            placed = true;
        }

        if (!placed)
        {
            if (!entity.TryGet<DraggingFromSlotComponent>(out var draggingFromSlot) || draggingFromSlot.slotProvider == null)
            {
                Debug.LogWarning("No original slot found for return.");
            }
            else
            {
                var originalSlot = draggingFromSlot.slotProvider;
                ref var abilitySlot = ref originalSlot.Entity.Get<AbilitySlotComponent>();
                if (!abilitySlot.itemEntity.IsAlive)
                {
                    originalSlot.AddItem(view);
                }
                else
                {
                    Debug.LogWarning("Original slot is occupied, cannot return item.");
                }
            }
        }

        view.EnableCollider(true);
        entity.Remove<IsDraggingAbility>();
        entity.Remove<PhysicsDragComponent>();
        entity.Remove<DraggingFromSlotComponent>();
    }
}
