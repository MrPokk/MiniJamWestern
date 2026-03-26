using System;
using BitterECS.Core;
using UnityEngine;

public class EnemyIntentOutlineSystem : IEcsRunSystem, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagEnemy> _filter;
    private EcsEvent _intentEvent;

    public void Init()
    {
        _intentEvent.Subscribe<IsIntentComponent>(removed: OnIntentRemoved);
    }

    private void OnIntentRemoved(EcsEntity entity)
    {
        ResetIntentDisplaySystem.OnReset(entity);
        IntentVisualUtility.ClearVisuals(entity);
    }

    public void Run()
    {
        _filter.For((EcsEntity entity, ref TagEnemy _) =>
        {
            if (!entity.IsAlive || entity.Has<IsDeadEvent>() || entity.Has<IsPreDestroyDeadEvent>()) return;

            if (entity.TryGet<EnemyStateComponent>(out var state) && state.state != EnemyState.Thinking && entity.TryGet<IsIntentComponent>(out var intent))
            {
                var color = Color.white;
                if (intent.abilityEntity.IsAlive && intent.abilityEntity.TryGet<SetColorComponent>(out var colorComp))
                {
                    color = colorComp.color;
                }

                if (entity.TryGet<OutlineComponent>(out var outlineComp))
                {
                    outlineComp.SetOutlineColor(color);
                }
            }
        });
    }
}
