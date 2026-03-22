using BitterECS.Core;
using UnityEngine;

public class EnemyIntentDisplaySystem : IEcsRunSystem
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<IsIntentComponent, EnemyStateComponent, TagEnemy> _filter;
    private EcsEvent _intentEvent = new EcsEvent().Subscribe<IsIntentComponent>(removed: OnIntentRemoved);

    public void Run()
    {
        _filter.For((EcsEntity entity, ref IsIntentComponent intent, ref EnemyStateComponent state, ref TagEnemy _) =>
        {
            if (state.state == EnemyState.Thinking) return;

            var worldPos = GridInteractionHandler.Instance._playfield.ConvertingPosition(intent.targetPosition);
            var color = Color.white;

            if (intent.abilityEntity.IsAlive && intent.abilityEntity.TryGet<SetColorComponent>(out var colorComp))
            {
                color = colorComp.color;
            }

            DrawRectUtility.Instance?.DrawStaticRect(entity.GetHashCode(), worldPos, 32f, color);
        });
    }

    private static void OnIntentRemoved(EcsEntity entity)
    {
        DrawRectUtility.Instance?.HideStaticRect(entity.GetHashCode());
    }
}
