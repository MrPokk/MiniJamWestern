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

            // Просто обновляем целевую позицию
            ref var physicsDrag = ref entity.Get<PhysicsDragComponent>();
            physicsDrag.targetWorldPosition = worldPos;
        }
    }
}
