using System;
using BitterECS.Core;
using UnityEngine;

public class EnemyIntentDisplaySystem : IEcsRunSystem, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<IsIntentComponent, EnemyStateComponent, TagEnemy> _filter;
    private EcsEvent _intentEvent;

    public void Init()
    {
        _intentEvent.Subscribe<IsIntentComponent>(removed: OnIntentRemoved);
    }

    private void OnIntentRemoved(EcsEntity entity)
    {
        ResetIntentDisplaySystem.OnReset(entity);
    }

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

            if (entity.TryGet<OutlineComponent>(out var outlineComp))
            {
                outlineComp.SetOutlineColor(color);
            }

            DrawRectUtility.Instance?.DrawStaticRect(entity.GetHashCode(), worldPos, 32f, color);
        });
    }

}
