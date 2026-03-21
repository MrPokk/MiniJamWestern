using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class AbilityHoverSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent().Subscribe<IsHoverAbility>(removed: OnResetColor);

    private static EcsFilter<TagSelector, RenderSpriteComponent> _ecsEntitiesSelector;
    private EcsFilter<IsHoverAbility, SetColorComponent> _ecsEntities;
    private static EcsFilter<OutlineComponent, GridComponent, TagPlayer> _ecsEntitiesOutline;

    public void Run()
    {
        _ecsEntities.For((EcsEntity ecsEntity, ref IsHoverAbility hover, ref SetColorComponent colorComponent) =>
        {
            var colorCom = colorComponent;

            _ecsEntitiesOutline.For((EcsEntity entity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer _) =>
            {
                outline.SetOutlineColor(colorCom.color);
            });

            _ecsEntitiesSelector.For((EcsEntity selectorEntity, ref TagSelector selector, ref RenderSpriteComponent spriteComponent) =>
            {
                if (spriteComponent.renderer != null)
                {
                    spriteComponent.renderer.color = colorCom.color;
                }

                var provider = selectorEntity.GetProvider<TagSelectorProvider>();
                provider.gameObject.SetActive(true);

                EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();
                var targetTo = _ecsEntitiesOutline.First().GetOrAdd<TargetTo>();

                if (provider != null)
                {
                    provider.transform.position = GridInteractionHandler.Instance._playfield.ConvertingPosition(targetTo.position);
                }
            });
        });
    }

    private static void OnResetColor(EcsEntity entity)
    {
        _ecsEntitiesOutline.For((EcsEntity playerEntity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer _) =>
        {
            outline.SetOutlineColor(outline.defaultOutlineColor);
        });

        _ecsEntitiesSelector.For((EcsEntity selectorEntity, ref TagSelector selector, ref RenderSpriteComponent spriteComponent) =>
        {
            if (spriteComponent.renderer != null)
            {
                spriteComponent.renderer.color = Color.white;
            }
            var provider = selectorEntity.GetProvider<TagSelectorProvider>();
            provider.gameObject.SetActive(false);

        });
    }
}
