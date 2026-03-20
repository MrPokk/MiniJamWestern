using BitterECS.Core;
using UnityEngine;

public class AbilityPhysicsDragSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<PhysicsDragComponent> _filter = new();

    private const float SPRING = 200f;
    private const float DAMPER = 15f;

    public void Run()
    {
        var dt = Mathf.Min(Time.deltaTime, 0.05f);

        _filter.For((EcsEntity entity, ref PhysicsDragComponent physics) =>
        {
            var view = entity.GetProvider<AbilityViewProvider>();
            if (view == null) return;

            var currentPos = view.transform.position;
            var displacement = physics.targetWorldPosition - currentPos;

            var force = (displacement * SPRING) - (physics.currentVelocity * DAMPER);

            physics.currentVelocity += force * dt;
            view.transform.position += physics.currentVelocity * dt;
        });
    }
}
