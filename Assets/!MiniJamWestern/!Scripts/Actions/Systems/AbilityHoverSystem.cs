using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class AbilityHoverSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;
    private EcsEvent _ecsEvent = new EcsEvent().Subscribe<IsHoverAbility>(removed: OnResetColor);
    private EcsFilter<IsHoverAbility, SetColorComponent> _ecsEntities;
    private static EcsFilter<OutlineComponent, GridComponent, TagPlayer> _ecsEntitiesOutline;
    private EcsFilter<TagInventoryEffects> _storageFilter;


    public void Run()
    {
        _ecsEntities.For((EcsEntity abilityEntity, ref IsHoverAbility hover, ref SetColorComponent colorComponent) =>
        {
            var targetColor = colorComponent.color;

            _ecsEntitiesOutline.For((EcsEntity playerEntity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer _) =>
            {
                outline.SetOutlineColor(targetColor);

                EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();

                if (!playerEntity.TryGet<TargetTo>(out var targetTo)) return;

                IntentVisualUtility.DrawAbilityArea(
                    playerEntity,
                    abilityEntity,
                    grid.currentPosition,
                    targetTo.position,
                    targetColor,
                    "Hover".GetHashCode());
            });
        });
    }

    private static void OnResetColor(EcsEntity _)
    {
        _ecsEntitiesOutline.For((EcsEntity playerEntity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer __) =>
        {
            outline.SetOutlineColor(outline.defaultOutlineColor);
            IntentVisualUtility.ClearVisuals(playerEntity);
        });
    }
}
