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
            if (!TryDrawVisuals(entity))
            {
                HideVisuals(entity);
            }
        });
    }

    private bool TryDrawVisuals(EcsEntity entity)
    {
        if (!entity.IsAlive || entity.Has<IsDeadEvent>() || entity.Has<IsPreDestroyDeadEvent>())
            return false;

        if (!entity.TryGet<EnemyStateComponent>(out var state) || state.state == EnemyState.Thinking)
            return false;

        if (!entity.TryGet<IsIntentComponent>(out var intent) || !entity.TryGet<GridComponent>(out var grid))
            return false;

        var abilityEntity = intent.abilityEntity;
        if (!abilityEntity.IsAlive)
            return false;

        var color = abilityEntity.TryGet<SetColorComponent>(out var colorComp) ? colorComp.color : Color.white;

        // Отрисовка сетки
        IntentVisualUtility.DrawAbilityArea(entity, abilityEntity, grid.currentPosition, intent.targetPosition, color);

        UpdateInteractiveIcon(entity, intent, color);

        return true;
    }

    private void UpdateInteractiveIcon(EcsEntity entity, IsIntentComponent intent, Color color)
    {
        if (!entity.TryGet<InteractiveIconComponent>(out var icon))
            return;

        icon.iconSprite.gameObject.SetActive(true);

        icon.iconSprite.spriteRenderer.color = color;
        icon.iconSprite.spriteRenderer.sprite = intent.chosenAbility switch
        {
            IAttackAbility _ => icon.attack,
            IMoveAbility _ => icon.move,
            IRotationAbility _ => icon.rotation,
            _ => icon.iconSprite.spriteRenderer.sprite
        };
    }

    private void HideVisuals(EcsEntity entity)
    {
        if (entity.TryGet<IntentVisualTracker>(out var tracker) && tracker.activeRectIds?.Count > 0)
        {
            IntentVisualUtility.ClearVisuals(entity);
        }

        if (entity.TryGet<InteractiveIconComponent>(out var icon))
        {
            icon.iconSprite.gameObject.SetActive(false);
        }
    }
}
