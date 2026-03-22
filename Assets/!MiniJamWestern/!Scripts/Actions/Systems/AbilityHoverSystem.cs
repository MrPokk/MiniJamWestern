using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class AbilityHoverSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent().Subscribe<IsHoverAbility>(removed: OnResetColor);

    private EcsFilter<IsHoverAbility, SetColorComponent> _ecsEntities;
    private static EcsFilter<OutlineComponent, GridComponent, TagPlayer> _ecsEntitiesOutline;

    public void Run()
    {
        _ecsEntities.For((EcsEntity ecsEntity, ref IsHoverAbility hover, ref SetColorComponent colorComponent) =>
        {
            var targetColor = colorComponent.color;

            _ecsEntitiesOutline.For((EcsEntity entity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer _) =>
            {
                outline.SetOutlineColor(targetColor);
            });

            EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();

            var playerEntity = _ecsEntitiesOutline.First();
            if (playerEntity.Has<TargetTo>())
            {
                var targetTo = playerEntity.Get<TargetTo>();
                var worldPos = GridInteractionHandler.Instance._playfield.ConvertingPosition(targetTo.position);

                if (DrawRectUtility.Instance != null)
                {
                    DrawRectUtility.Instance.DrawStaticRect(worldPos, 32f, targetColor);
                }
            }
        });
    }

    private static void OnResetColor(EcsEntity entity)
    {
        _ecsEntitiesOutline.For((EcsEntity playerEntity, ref OutlineComponent outline, ref GridComponent grid, ref TagPlayer _) =>
        {
            outline.SetOutlineColor(outline.defaultOutlineColor);
        });

        if (DrawRectUtility.Instance != null)
        {
            DrawRectUtility.Instance.HideRect();
        }
    }
}
