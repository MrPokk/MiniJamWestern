using System;
using BitterECS.Core;

public class AbilityHoverSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent().Subscribe<IsHoverAbility>(removed: OnResetColor);

    private EcsFilter<IsHoverAbility, SetColorComponent> _ecsEntities;
    private static EcsFilter<OutlineComponent, TagPlayer> _ecsEntitiesOutline;

    public void Run()
    {
        _ecsEntities.For((EcsEntity ecsEntity, ref IsHoverAbility hover, ref SetColorComponent colorComponent) =>
        {
            var colorCom = colorComponent;
            _ecsEntitiesOutline.For((EcsEntity entity, ref OutlineComponent outline, ref TagPlayer _) =>
            {
                outline.SetOutlineColor(colorCom.color);
            });
        });
    }

    private static void OnResetColor(EcsEntity entity)
    {
        _ecsEntitiesOutline.For((EcsEntity entity, ref OutlineComponent outline, ref TagPlayer _) =>
        {
            outline.SetOutlineColor(outline.defaultOutlineColor);
        });
    }
}
