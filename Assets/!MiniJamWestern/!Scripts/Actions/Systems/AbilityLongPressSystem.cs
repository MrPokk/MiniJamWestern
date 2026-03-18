using BitterECS.Core;
using UnityEngine;

public class AbilityLongPressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<LongPressAbilityEvent>(added: OnLongPress);

    private static void OnLongPress(EcsEntity abilityEntity)
    {
        var view = abilityEntity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        var parentSlot = view.GetComponentInParent<AbilitySlotProvider>();

        if (parentSlot != null && parentSlot.Value.itemEntity == abilityEntity)
        {
            parentSlot.TryRemoveItem();
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
