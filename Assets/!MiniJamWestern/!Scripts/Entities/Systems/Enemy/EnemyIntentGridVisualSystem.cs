using BitterECS.Core;
using UnityEngine;

public class EnemyIntentGridVisualSystem : IEcsRunSystem
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagEnemy> _filter;

    public void Run()
    {
        _filter.For((EcsEntity entity, ref TagEnemy _) =>
        {
            var shouldDraw = false;

            // Если враг жив и не умирает
            if (entity.IsAlive && !entity.Has<IsDeadEvent>() && !entity.Has<IsPreDestroyDeadEvent>())
            {
                if (entity.TryGet<EnemyStateComponent>(out var state) && state.state != EnemyState.Thinking)
                {
                    if (entity.TryGet<IsIntentComponent>(out var intent) && entity.TryGet<GridComponent>(out var grid))
                    {
                        var abilityEntity = intent.abilityEntity;
                        if (abilityEntity.IsAlive)
                        {
                            shouldDraw = true;

                            var color = Color.white;
                            if (abilityEntity.TryGet<SetColorComponent>(out var colorComp))
                                color = colorComp.color;

                            IntentVisualUtility.DrawAbilityArea(entity, abilityEntity, grid.currentPosition, intent.targetPosition, color);
                        }
                    }
                }
            }

            if (!shouldDraw)
            {
                if (entity.TryGet<IntentVisualTracker>(out var tracker) && tracker.activeRectIds != null && tracker.activeRectIds.Count > 0)
                {
                    IntentVisualUtility.ClearVisuals(entity);
                }
            }
        });
    }
}
