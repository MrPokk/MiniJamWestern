using BitterECS.Core;
using UnityEngine;

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
