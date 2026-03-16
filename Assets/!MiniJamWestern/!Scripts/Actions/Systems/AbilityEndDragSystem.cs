using BitterECS.Core;
using UnityEngine;

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
        else
        {
            entity.Remove<IsDraggingAbility>();
            view.EnableCollider(true);
        }
    }
}
