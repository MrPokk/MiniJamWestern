using BitterECS.Core;
using UnityEngine;

public class EnemyIntentGridVisualSystem : IEcsRunSystem
{
    public Priority Priority => Priority.Medium;
    private EcsFilter<IsIntentComponent, EnemyStateComponent, TagEnemy, GridComponent> _filter;

    public void Run()
    {
        _filter.For((EcsEntity entity, ref IsIntentComponent intent, ref EnemyStateComponent state, ref TagEnemy _, ref GridComponent grid) =>
        {
            if (state.state == EnemyState.Thinking) return;

            var abilityEntity = intent.abilityEntity;
            if (!abilityEntity.IsAlive) return;

            var color = Color.white;
            if (abilityEntity.TryGet<SetColorComponent>(out var colorComp))
                color = colorComp.color;

            IntentVisualUtility.DrawAbilityArea(entity, abilityEntity, grid.currentPosition, intent.targetPosition, color);
        });
    }
}
