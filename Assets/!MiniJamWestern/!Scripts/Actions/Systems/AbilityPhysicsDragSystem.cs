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
        // Защита от скачков физики при лагах
        float dt = Mathf.Min(Time.deltaTime, 0.05f);

        _filter.For((EcsEntity entity, ref PhysicsDragComponent physics) =>
        {
            var view = entity.GetProvider<AbilityViewProvider>();
            if (view == null) return;

            Vector3 currentPos = view.transform.position;
            Vector3 displacement = physics.targetWorldPosition - currentPos;

            // Формула пружинного маятника
            Vector3 force = (displacement * SPRING) - (physics.currentVelocity * DAMPER);

            physics.currentVelocity += force * dt;
            view.transform.position += physics.currentVelocity * dt;
        });
    }
}
